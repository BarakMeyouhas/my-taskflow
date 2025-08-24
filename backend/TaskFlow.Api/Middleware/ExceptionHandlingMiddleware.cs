using System.Net;
using System.Text.Json;

namespace TaskFlow.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// This middleware catches any unhandled exceptions and returns a proper error response
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment
        )
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Determine the appropriate status code
            var statusCode = exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };

            context.Response.StatusCode = (int)statusCode;

            // Create error response
            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = _environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request.",
                Details = _environment.IsDevelopment() ? exception.StackTrace : null,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path,
            };

            // Serialize and send the error response
            var jsonResponse = JsonSerializer.Serialize(
                errorResponse,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extension method to register the exception handling middleware
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
