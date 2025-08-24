using Microsoft.AspNetCore.Http;

namespace TaskFlow.Api.Middleware
{
    /// <summary>
    /// Example middleware class that demonstrates how to create reusable middleware
    /// This middleware logs detailed information about each HTTP request
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Log request details
                _logger.LogInformation(
                    "Request started: {Method} {Path} from {IP}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress
                );

                // Store start time
                var startTime = DateTime.UtcNow;

                // Call the next middleware in the pipeline
                await _next(context);

                // Calculate duration
                var duration = DateTime.UtcNow - startTime;

                // Log response details
                _logger.LogInformation(
                    "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    duration.TotalMilliseconds
                );
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during request processing
                _logger.LogError(
                    ex,
                    "Request failed: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path
                );

                // Re-throw the exception to maintain the error handling pipeline
                throw;
            }
        }
    }

    /// <summary>
    /// Extension method to make it easy to register this middleware
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
