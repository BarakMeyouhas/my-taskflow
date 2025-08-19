using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface IQueueService
    {
        Task<bool> SendUserRegistrationMessageAsync(User user, string password);
        Task<bool> SendMessageAsync<T>(string queueName, T message);
        bool IsAvailable();
    }

    public class QueueService : IQueueService
    {
        private readonly QueueServiceClient? _queueServiceClient;
        private readonly ILogger<QueueService> _logger;
        private readonly bool _isAvailable;
        private const string UserRegistrationQueueName = "user-registration-queue";

        public QueueService(IConfiguration configuration, ILogger<QueueService> logger)
        {
            _logger = logger;
            
            // Try to get connection string from environment variable first, then fallback to configuration
            var connectionString =
                Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
                ?? configuration.GetConnectionString("AzureStorage")
                ?? configuration["AzureWebJobsStorage"];

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Azure Storage connection string is not configured. Queue service will be unavailable.");
                _isAvailable = false;
                _queueServiceClient = null;
                return;
            }

            try
            {
                _queueServiceClient = new QueueServiceClient(connectionString);
                _isAvailable = true;
                _logger.LogInformation("QueueService initialized successfully with Azure Storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize QueueService with Azure Storage");
                _isAvailable = false;
                _queueServiceClient = null;
            }
        }

        public bool IsAvailable()
        {
            return _isAvailable && _queueServiceClient != null;
        }

        public async Task<bool> SendUserRegistrationMessageAsync(User user, string password)
        {
            if (!IsAvailable())
            {
                _logger.LogWarning("Queue service is not available. Cannot send user registration message.");
                return false;
            }

            try
            {
                var message = new
                {
                    Username = user.Username,
                    Email = user.Email,
                    Password = password,
                    RequestedAt = DateTime.UtcNow,
                    RequestId = Guid.NewGuid().ToString(),
                };

                return await SendMessageAsync(UserRegistrationQueueName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending user registration message to queue for user: {Username}",
                    user.Username
                );
                return false;
            }
        }

        public async Task<bool> SendMessageAsync<T>(string queueName, T message)
        {
            if (!IsAvailable())
            {
                _logger.LogWarning("Queue service is not available. Cannot send message to queue: {QueueName}", queueName);
                return false;
            }

            try
            {
                var queueClient = _queueServiceClient!.GetQueueClient(queueName);

                // Ensure queue exists
                await queueClient.CreateIfNotExistsAsync();

                // Serialize message to JSON
                var messageJson = JsonSerializer.Serialize(message);
                var messageBytes = System.Text.Encoding.UTF8.GetBytes(messageJson);

                // Send message to queue
                await queueClient.SendMessageAsync(Convert.ToBase64String(messageBytes));

                _logger.LogInformation(
                    "Message sent successfully to queue: {QueueName}",
                    queueName
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue: {QueueName}", queueName);
                return false;
            }
        }
    }
}
