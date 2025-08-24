using System.Data;
using System.Text;
using Azure.Storage.Queues;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api.Data;
using TaskFlow.Api.Middleware;
using TaskFlow.Api.Services;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Get connection string from configuration (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Connection string is now directly from appsettings.json

// Log connection string info (without sensitive data)
var connectionInfo =
    connectionString?.Contains("barak.database.windows.net") == true
        ? "Using Azure SQL Database connection"
        : "Using fallback connection string";
Console.WriteLine($"Database Connection: {connectionInfo}");

// Debug logging for connection string processing
Console.WriteLine($"=== DATABASE CONNECTION DEBUG ===");
Console.WriteLine($"Using connection string from appsettings.json");
Console.WriteLine(
    $"Connection string (masked): {connectionString?.Replace("Kingzerz@1998", "***")}"
);
Console.WriteLine($"=== END DATABASE DEBUG ===");

// Validate database connection string
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: No database connection string found!");
    Console.WriteLine("Please configure DefaultConnection in appsettings.json");
}

// Get Azure Storage connection string from configuration (appsettings.json)
var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");

// Azure Storage connection string is now directly from appsettings.json

if (string.IsNullOrEmpty(azureStorageConnectionString))
{
    Console.WriteLine("ERROR: No Azure Storage connection string found!");
    Console.WriteLine("Please configure AzureStorage in appsettings.json");
}

Console.WriteLine(
    $"Azure Storage Connection: {(string.IsNullOrEmpty(azureStorageConnectionString) ? "NOT CONFIGURED" : "Configured")}"
);

// Add database context with retry logic (Entity Framework)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    )
);

// Add Dapper connection (for services that use Dapper)
builder.Services.AddScoped<IDbConnection>(provider =>
{
    var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    return connection;
});

// Register Azure Storage services
if (!string.IsNullOrEmpty(azureStorageConnectionString))
{
    try
    {
        // Register QueueServiceClient as singleton
        builder.Services.AddSingleton<QueueServiceClient>(provider =>
        {
            return new QueueServiceClient(azureStorageConnectionString);
        });
        Console.WriteLine("QueueServiceClient registered successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: Failed to register QueueServiceClient: {ex.Message}");
    }
}
else
{
    // Register null QueueServiceClient when no connection string is available
    builder.Services.AddSingleton<QueueServiceClient>(_ => null!);
    Console.WriteLine("QueueServiceClient registered as null (no connection string)");
}

// Register application services
try
{
    // Core services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IGreetingService, GreetingServiceDapper>(); // Now using Dapper version

    // Queue service (depends on QueueServiceClient)
    builder.Services.AddScoped<IQueueService, QueueService>();

    Console.WriteLine("All application services registered successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: Failed to register application services: {ex.Message}");
}

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddOpenApi();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(jwtKey))
        {
            // Fallback to configuration if environment variable not set
            jwtKey = builder.Configuration["JwtSettings:Key"];
            // Substitute environment variable placeholder if present
            if (jwtKey?.Contains("${JWT_KEY}") == true)
            {
                var envJwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
                if (!string.IsNullOrEmpty(envJwtKey))
                {
                    jwtKey = envJwtKey;
                }
            }
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "taskflow",
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "taskflow",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtKey ?? "SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345"
                )
            ),
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

// ===== MIDDLEWARE EXAMPLES - Learning about middleware concepts =====

// Global exception handling middleware (should be one of the first middleware in the pipeline)
app.UseGlobalExceptionHandler();

// Request logging middleware (logs detailed information about each request)
app.UseRequestLogging();

// Custom headers middleware - Adds custom headers to responses
app.Use(
    async (context, next) =>
    {
        // Add custom headers before calling next middleware
        context.Response.Headers.Append("X-Powered-By", "TaskFlow-API");
        context.Response.Headers.Append("X-Request-ID", Guid.NewGuid().ToString());

        await next();
    }
);

// Simple authentication check middleware (for demonstration)
app.Use(
    async (context, next) =>
    {
        // Skip authentication for health check and test endpoints
        if (
            context.Request.Path.StartsWithSegments("/")
            || context.Request.Path.StartsWithSegments("/test")
            || context.Request.Path.StartsWithSegments("/weatherforecast")
        )
        {
            await next();
            return;
        }

        // For other endpoints, check if Authorization header exists
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authorization header required");
            return;
        }

        await next();
    }
);

// ===== END MIDDLEWARE EXAMPLES =====

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// בריאות בסיסית ל־root "/"
app.MapGet(
    "/",
    (IQueueService queueService) =>
        Results.Ok(
            new
            {
                status = "OK",
                message = "TaskFlow API is running 🚀",
                timestamp = DateTime.UtcNow,
                environment = app.Environment.EnvironmentName,
                databaseConfigured = !string.IsNullOrEmpty(connectionString),
                azureStorageConfigured = !string.IsNullOrEmpty(azureStorageConnectionString),
                azureStorageAvailable = queueService.IsAvailable(),
                note = "Database context enabled with retry logic",
            }
        )
);

// Test endpoint to verify routing
app.MapGet("/test", () => Results.Ok(new { message = "Test endpoint working!" }));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
