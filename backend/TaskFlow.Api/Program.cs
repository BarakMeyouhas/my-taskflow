using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api.Data;
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

// Get connection string from environment variable or fallback to configuration
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Substitute environment variables in connection string if needed
if (connectionString?.Contains("${DATABASE_PASSWORD}") == true)
{
    var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
    if (!string.IsNullOrEmpty(dbPassword))
    {
        connectionString = connectionString.Replace("${DATABASE_PASSWORD}", dbPassword);
    }
}

// Log connection string info (without sensitive data)
var connectionInfo =
    connectionString?.Contains("barak.database.windows.net") == true
        ? "Using Azure SQL Database connection"
        : "Using fallback connection string";
Console.WriteLine($"Database Connection: {connectionInfo}");

// Debug logging for connection string processing
Console.WriteLine($"=== DATABASE CONNECTION DEBUG ===");
Console.WriteLine(
    $"DATABASE_CONNECTION_STRING env var: {(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") != null ? "SET" : "NOT SET")}"
);
Console.WriteLine(
    $"DATABASE_PASSWORD env var: {(Environment.GetEnvironmentVariable("DATABASE_PASSWORD") != null ? "SET" : "NOT SET")}"
);
Console.WriteLine(
    $"Connection string contains placeholder: {connectionString?.Contains("${DATABASE_PASSWORD}")}"
);
Console.WriteLine(
    $"Final connection string (masked): {connectionString?.Replace(Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "MASKED", "***")}"
);
Console.WriteLine($"=== END DATABASE DEBUG ===");

// Validate database connection string
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: No database connection string found!");
    Console.WriteLine(
        "Please set DATABASE_CONNECTION_STRING environment variable or configure DefaultConnection in appsettings.json"
    );
}

// Validate Azure Storage connection string
var azureStorageConnectionString =
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("AzureStorage")
    ?? builder.Configuration["AzureWebJobsStorage"];

// Substitute environment variables in Azure Storage connection string if needed
if (azureStorageConnectionString?.Contains("${AZURE_STORAGE_ACCOUNT_KEY}") == true)
{
    var storageKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
    if (!string.IsNullOrEmpty(storageKey))
    {
        azureStorageConnectionString = azureStorageConnectionString.Replace(
            "${AZURE_STORAGE_ACCOUNT_KEY}",
            storageKey
        );
    }
}

if (string.IsNullOrEmpty(azureStorageConnectionString))
{
    Console.WriteLine("ERROR: No Azure Storage connection string found!");
    Console.WriteLine(
        "Please set AZURE_STORAGE_CONNECTION_STRING environment variable or configure AzureStorage in appsettings.json"
    );
}

Console.WriteLine(
    $"Azure Storage Connection: {(string.IsNullOrEmpty(azureStorageConnectionString) ? "NOT CONFIGURED" : "Configured")}"
);

// Add database context with retry logic
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

// Temporarily disable QueueService for testing startup
// try
// {
//     builder.Services.AddScoped<IQueueService, QueueService>();
//     Console.WriteLine("QueueService registered successfully");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"ERROR: Failed to register QueueService: {ex.Message}");
//     // Continue without queue service for now
// }

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

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// בריאות בסיסית ל־root "/"
app.MapGet(
    "/",
    () =>
        Results.Ok(
            new
            {
                status = "OK",
                message = "TaskFlow API is running 🚀",
                timestamp = DateTime.UtcNow,
                environment = app.Environment.EnvironmentName,
                databaseConfigured = !string.IsNullOrEmpty(connectionString),
                azureStorageConfigured = !string.IsNullOrEmpty(azureStorageConnectionString),
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
