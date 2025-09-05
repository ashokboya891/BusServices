using API.Helpers;
using BusServcies.IServiceContracts;
using BusServcies.Services;
using BusServices.IRepositoryContracts;
using BusServices.IServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            var configuration = builder.Configuration.GetConnectionString("Redis"); // 👈 Add in appsettings.json
            return ConnectionMultiplexer.Connect(configuration);
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379"; // Redis server URL
            options.InstanceName = "BusServcie_"; // Optional prefix for keys
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<BusServcies.IServiceContracts.IResponseCacheService,BusServcies.ServiceContracts.ReponseCacheService>();

        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IEventRepository, BusServices.RepositoryContracts.EventRepository>();
        builder.Services.AddScoped<IEventService, BusServices.ServiceContracts.EventServcie>();

        // Add the CORS policy to the application
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
            {
                policyBuilder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                            .AllowCredentials(); // ✅ REQUIRED for SignalR 


            });
        });
        // Configure JWT authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer("Bearer", options =>
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
                    {
                        securitySchema,new[] {"Bearer"}
                    }
                                };
            c.AddSecurityRequirement(securityRequirement);
        }); ;
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

        app.UseCors("AllowSpecificOrigins"); // Apply CORS policy

        app.UseAuthentication(); // ✅ Must come before Authorization
        app.UseAuthorization();  // ✅ You missed this

        app.MapControllers();    // ✅ Endpoints mapping comes after auth middlewares

        app.Run();
    }
}