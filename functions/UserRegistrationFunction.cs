using System.Text.Json;
using Azure.Storage.Queues.Models;
using BCrypt.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.Functions.Data;
using TaskFlow.Functions.Models;

namespace TaskFlow.Functions
{
    public class UserRegistrationFunction
    {
        private readonly ILogger<UserRegistrationFunction> _logger;
        private readonly FunctionsDbContext _dbContext;

        public UserRegistrationFunction(
            ILogger<UserRegistrationFunction> logger,
            FunctionsDbContext dbContext
        )
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function("UserRegistration")]
        public async Task Run(
            [QueueTrigger("user-registration-queue", Connection = "AzureWebJobsStorage")]
                QueueMessage message
        )
        {
            try
            {
                _logger.LogInformation(
                    "Processing user registration message: {messageText}",
                    message.MessageText
                );

                // Deserialize the message from JSON
                var registrationMessage = JsonSerializer.Deserialize<UserRegistrationMessage>(
                    message.MessageText
                );

                if (registrationMessage == null)
                {
                    _logger.LogError("Failed to deserialize user registration message");
                    return;
                }

                _logger.LogInformation(
                    "Processing user registration for username: {Username}",
                    registrationMessage.Username
                );
                _logger.LogInformation("Request ID: {RequestId}", registrationMessage.RequestId);

                // Check if user already exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                    u.Username == registrationMessage.Username
                    || u.Email == registrationMessage.Email
                );

                if (existingUser != null)
                {
                    _logger.LogWarning(
                        "User registration failed: Username or email already exists. Username: {Username}, Email: {Email}",
                        registrationMessage.Username,
                        registrationMessage.Email
                    );
                    return;
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationMessage.Password);

                // Create new user
                var newUser = new User
                {
                    Username = registrationMessage.Username,
                    Email = registrationMessage.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                };

                // Add to database
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "User registration successful for username: {Username}, User ID: {UserId}",
                    registrationMessage.Username,
                    newUser.Id
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing user registration message: {MessageText}",
                    message.MessageText
                );
                throw; // Re-throw to trigger retry mechanism
            }
        }
    }
}
