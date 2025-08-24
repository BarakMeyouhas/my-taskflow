# Middleware Concepts in ASP.NET Core

This folder contains examples of middleware components to help you understand how middleware works in ASP.NET Core APIs.

## What is Middleware?

Middleware is software that's assembled into an application pipeline to handle requests and responses. Each middleware component:

1. **Chooses whether to pass the request to the next component in the pipeline**
2. **Can perform work before and after the next component**
3. **Can short-circuit the pipeline by not calling the next component**

## Middleware Pipeline Flow

```
Request → Middleware 1 → Middleware 2 → Middleware 3 → Endpoint → Response
         ↑            ↑              ↑              ↑
         Logging      Auth           Headers        Controller
```

## Examples Included

### 1. ExceptionHandlingMiddleware
- **Purpose**: Catches unhandled exceptions globally
- **Position**: Should be one of the first middleware in the pipeline
- **Features**: 
  - Logs exceptions
  - Returns structured error responses
  - Different behavior for development vs production
  - Maps exceptions to appropriate HTTP status codes

### 2. RequestLoggingMiddleware
- **Purpose**: Logs detailed information about each request
- **Features**:
  - Request start/end logging
  - Performance timing
  - IP address tracking
  - Structured logging with Serilog-style placeholders

### 3. Inline Middleware Examples
- **Custom Headers**: Adds `X-Powered-By` and `X-Request-ID` headers
- **Simple Auth Check**: Demonstrates basic authentication validation

## How to Use

### Registering Middleware
```csharp
// In Program.cs
app.UseGlobalExceptionHandler();        // Custom middleware class
app.UseRequestLogging();               // Custom middleware class
app.Use(async (context, next) => { }); // Inline middleware
```

### Middleware Order Matters
The order you register middleware determines the execution order:

1. **Exception handling** should be first
2. **Logging** should be early
3. **Authentication/Authorization** should be before endpoints
4. **Custom headers** can be anywhere in the middle
5. **Endpoints** should be last

### Creating Custom Middleware

#### Option 1: Inline Middleware
```csharp
app.Use(async (context, next) =>
{
    // Do something before the request
    Console.WriteLine("Before request");
    
    // Call the next middleware
    await next();
    
    // Do something after the request
    Console.WriteLine("After request");
});
```

#### Option 2: Middleware Class
```csharp
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    
    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Before logic
        await _next(context);
        // After logic
    }
}

// Extension method for easy registration
public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomMiddleware>();
    }
}
```

## Key Concepts

### 1. RequestDelegate
- Represents the next middleware in the pipeline
- Must be called to continue the pipeline
- Can be called multiple times (though usually not recommended)

### 2. HttpContext
- Contains all information about the current HTTP request/response
- Accessible throughout the entire pipeline
- Can be modified by any middleware

### 3. Pipeline Short-Circuiting
```csharp
// Don't call next() to stop the pipeline
if (unauthorized)
{
    context.Response.StatusCode = 401;
    await context.Response.WriteAsync("Unauthorized");
    return; // Pipeline stops here
}

await next(); // Continue pipeline
```

### 4. Middleware Dependencies
- Middleware can depend on services registered in DI container
- Use constructor injection to access services
- Middleware is instantiated once per application lifetime

## Testing Your Middleware

1. **Run the application** and make requests to different endpoints
2. **Check the console output** for logging information
3. **Inspect response headers** for custom headers
4. **Test error scenarios** to see exception handling
5. **Use tools like Postman** to test with/without Authorization headers

## Common Use Cases

- **Logging**: Request/response logging, performance monitoring
- **Authentication**: JWT validation, API key checking
- **Caching**: Response caching, distributed caching
- **Rate Limiting**: API throttling, abuse prevention
- **CORS**: Cross-origin resource sharing
- **Compression**: Response compression
- **Error Handling**: Global exception handling, custom error pages
- **Security**: HTTPS redirection, security headers
- **Monitoring**: Health checks, metrics collection

## Best Practices

1. **Keep middleware focused** on a single responsibility
2. **Handle exceptions properly** in middleware
3. **Use structured logging** for better debugging
4. **Consider performance impact** of middleware operations
5. **Test middleware thoroughly** with different scenarios
6. **Document middleware behavior** and configuration options
7. **Use extension methods** for clean registration
8. **Consider middleware order** carefully
