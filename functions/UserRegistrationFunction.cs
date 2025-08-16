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
                UserRegistrationMessage message
        )
        {
            try
            {
                _logger.LogInformation(
                    "Processing user registration for username: {Username}",
                    message.Username
                );
                _logger.LogInformation("Request ID: {RequestId}", message.RequestId);

                // Check if user already exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                    u.Username == message.Username || u.Email == message.Email
                );

                if (existingUser != null)
                {
                    _logger.LogWarning(
                        "User registration failed: Username or email already exists. Username: {Username}, Email: {Email}",
                        message.Username,
                        message.Email
                    );
                    return;
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(message.Password);

                // Create new user
                var newUser = new User
                {
                    Username = message.Username,
                    Email = message.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                };

                // Add to database
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "User registration successful for username: {Username}, User ID: {UserId}",
                    message.Username,
                    newUser.Id
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing user registration for username: {Username}, Request ID: {RequestId}",
                    message.Username,
                    message.RequestId
                );
                throw; // Re-throw to trigger retry mechanism
            }
        }
    }
}
