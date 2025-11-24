using API.Helpers;
using BusServcies.IServiceContracts;
using BusServcies.Middleware;
using BusServcies.Services;
using BusServices.IRepositoryContracts;
using BusServices.IServiceContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        // Add services to the container.

        builder.Services.AddControllers(opt =>
        {
            opt.Filters.Add(new ProducesAttribute("application/json"));
            opt.Filters.Add(new ConsumesAttribute("application/json"));
        }).AddXmlSerializerFormatters().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

        builder.Services.AddDbContext<BusServcies.DatabaseContext.ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = builder.Configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(configuration);
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
            options.InstanceName = "BusServcie_";
        });

        builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider Service, LoggerConfiguration config) =>
        {
            config.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(Service);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<BusServcies.IServiceContracts.IResponseCacheService, BusServcies.ServiceContracts.ReponseCacheService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IEventRepository, BusServices.RepositoryContracts.EventRepository>();
        builder.Services.AddScoped<IEventService, BusServices.ServiceContracts.EventServcie>();

        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
            {
                policyBuilder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                            .AllowCredentials();
            });
        });

        // ========== INTERNAL AUTHENTICATION SETUP ==========

        // Configure Authentication with Policy Scheme
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "InternalOrBearer";
            options.DefaultChallengeScheme = "InternalOrBearer";
        })
        // Add JWT Bearer Authentication
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        })
        // Add Internal Authentication Handler
        .AddScheme<AuthenticationSchemeOptions, InternalAuthHandler>("Internal", null)
        // Add Policy Scheme that decides between Internal and Bearer
        .AddPolicyScheme("InternalOrBearer", "InternalOrBearer", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                // If request has X-Internal-Key header, use Internal scheme
                if (context.Request.Headers.ContainsKey("X-Internal-Key"))
                {
                    return "Internal";
                }
                // If request has Authorization header with Bearer, use Bearer scheme
                if (context.Request.Headers.ContainsKey("Authorization") &&
                    context.Request.Headers["Authorization"].ToString().StartsWith("Bearer "))
                {
                    return "Bearer";
                }
                // Otherwise, let both schemes try
                return "Internal,Bearer";
            };
        });

        // Configure Authorization Policies
        builder.Services.AddAuthorization(options =>
        {
            // Policy that allows either Internal service OR users with Admin/User roles
            options.AddPolicy("InternalOrAdminUser", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    // Option 1: Internal service authentication
                    if (context.User.HasClaim(c => c.Type == "IsInternalService" && c.Value == "true"))
                    {
                        return true;
                    }
                    // Option 2: JWT authentication with Admin/User roles
                    if (context.User.IsInRole("Admin") || context.User.IsInRole("User"))
                    {
                        return true;
                    }
                    return false;
                });
            });

            // Additional policies for different role combinations
            options.AddPolicy("InternalOrAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    if (context.User.HasClaim(c => c.Type == "IsInternalService" && c.Value == "true"))
                        return true;
                    if (context.User.IsInRole("Admin"))
                        return true;
                    return false;
                });
            });

            // Policy for only internal services (no JWT users)
            options.AddPolicy("InternalOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(c => c.Type == "IsInternalService" && c.Value == "true");
                });
            });


        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "JWT Auth Bearer Scheme",
                Name = "Authorisation",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { "Bearer" } }
            };
            c.AddSecurityRequirement(securityRequirement);

            // Add support for Internal API Key in Swagger
            c.AddSecurityDefinition("InternalKey", new OpenApiSecurityScheme
            {
                Description = "Internal Service API Key",
                Name = "X-Internal-Key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Internal"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "InternalKey"
                        }
                    },
                    new string[] {}
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("AllowSpecificOrigins");

        // Enhanced debugging middleware
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"\n=== 🔐 AUTH DEBUG - {context.Request.Method} {context.Request.Path} ===");

            var hasInternalKey = context.Request.Headers.ContainsKey("X-Internal-Key");
            var hasAuthHeader = context.Request.Headers.ContainsKey("Authorization");

            Console.WriteLine($"Headers - InternalKey: {hasInternalKey}, AuthHeader: {hasAuthHeader}");

            if (hasInternalKey)
            {
                Console.WriteLine($"X-Internal-Key: {context.Request.Headers["X-Internal-Key"]}");
            }
            if (hasAuthHeader)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Authorization: {authHeader.Substring(0, Math.Min(50, authHeader.Length))}...");
            }

            await next();

            // After the request, log the auth status
            Console.WriteLine($"Response Status: {context.Response.StatusCode}");
            Console.WriteLine($"User Authenticated: {context.User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"Auth Type: {context.User?.Identity?.AuthenticationType}");
            Console.WriteLine($"User Name: {context.User?.Identity?.Name}");

            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var roles = context.User.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value);
                Console.WriteLine($"User Roles: {string.Join(", ", roles)}");

                var isInternal = context.User.HasClaim(c => c.Type == "IsInternalService");
                Console.WriteLine($"Is Internal Service: {isInternal}");
            }
            Console.WriteLine($"=== END AUTH DEBUG ===\n");
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.Run();
    }
}