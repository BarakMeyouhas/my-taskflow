using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace TaskFlow.Api.Services
{
    public interface IQueueService
    {
        Task<bool> SendMessageAsync<T>(string queueName, T message);
    }

    public class QueueService : IQueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            var queueName = "user-registration-queue";
            _queueClient = new QueueClient(connectionString, queueName);
        }

        public async Task<bool> SendMessageAsync<T>(string queueName, T message)
        {
            try
            {
                // Ensure queue exists
                await _queueClient.CreateIfNotExistsAsync();

                // Serialize message to JSON
                var jsonMessage = JsonSerializer.Serialize(message);
                var encodedMessage = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(jsonMessage)
                );

                // Send message to queue
                await _queueClient.SendMessageAsync(encodedMessage);

                return true;
            }
            catch (Exception ex)
            {
                // Log error (in production, use proper logging)
                Console.WriteLine($"Error sending message to queue: {ex.Message}");
                return false;
            }
        }
    }
}
