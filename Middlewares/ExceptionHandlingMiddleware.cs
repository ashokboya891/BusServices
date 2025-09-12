using BusServcies.Errors;
using System.Net;
using System.Text.Json;

namespace BusServcies.Middleware
{

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogException(ex); // custom logger method

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // In development → show detailed info
                // In production → safe message only
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException((int)HttpStatusCode.InternalServerError);

                //: new ApiException(context.Response.StatusCode, "An unexpected error occurred.", "Internal server error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }

        private void LogException(Exception ex)
        {
            // Logs deepest inner exception first (like your first version)
            if (ex.InnerException?.InnerException != null)
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.InnerException.GetType().ToString(), ex.InnerException.InnerException.Message);
            }
            else if (ex.InnerException != null)
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
            }
            else
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
            }
        }
    }

    // Response model
    //public class ApiException
    //{
    //    public ApiException(int statusCode, string message = null, string details = null)
    //    {
    //        StatusCode = statusCode;
    //        Message = message;
    //        Details = details;
    //    }

    //    public int? StatusCode { get; set; }
    //    public string Message { get; set; }
    //    public string Details { get; set; }
    //}
}
