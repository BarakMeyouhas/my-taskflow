using Microsoft.Extensions.Logging;

namespace TaskFlow.Functions.Services
{
    public interface IRetryService
    {
        Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            int maxRetries = 3,
            int delayMs = 1000
        );
        Task ExecuteWithRetryAsync(
            Func<Task> operation,
            string operationName,
            int maxRetries = 3,
            int delayMs = 1000
        );
    }

    public class RetryService : IRetryService
    {
        private readonly ILogger<RetryService> _logger;

        public RetryService(ILogger<RetryService> logger)
        {
            _logger = logger;
        }

        public async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            int maxRetries = 3,
            int delayMs = 1000
        )
        {
            var attempt = 0;
            Exception? lastException = null;

            while (attempt < maxRetries)
            {
                try
                {
                    attempt++;
                    _logger.LogInformation(
                        "Attempting {OperationName} (attempt {Attempt}/{MaxRetries})",
                        operationName,
                        attempt,
                        maxRetries
                    );

                    var result = await operation();

                    if (attempt > 1)
                    {
                        _logger.LogInformation(
                            "{OperationName} succeeded on attempt {Attempt}",
                            operationName,
                            attempt
                        );
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(
                        ex,
                        "Attempt {Attempt} of {OperationName} failed",
                        attempt,
                        operationName
                    );

                    if (attempt < maxRetries)
                    {
                        var delay = delayMs * attempt; // Exponential backoff
                        _logger.LogInformation("Waiting {Delay}ms before retry", delay);
                        await Task.Delay(delay);
                    }
                }
            }

            _logger.LogError(
                lastException,
                "{OperationName} failed after {MaxRetries} attempts",
                operationName,
                maxRetries
            );
            throw new InvalidOperationException(
                $"{operationName} failed after {maxRetries} attempts",
                lastException
            );
        }

        public async Task ExecuteWithRetryAsync(
            Func<Task> operation,
            string operationName,
            int maxRetries = 3,
            int delayMs = 1000
        )
        {
            await ExecuteWithRetryAsync(
                async () =>
                {
                    await operation();
                    return true;
                },
                operationName,
                maxRetries,
                delayMs
            );
        }
    }
}
