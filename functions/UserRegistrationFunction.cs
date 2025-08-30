using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TaskFlow.Functions.Models;
using TaskFlow.Functions.Services;

namespace TaskFlow.Functions
{
    public class UserRegistrationFunction
    {
        private readonly ILogger<UserRegistrationFunction> _logger;
        private readonly IEmailService _emailService;
        private readonly IRetryService _retryService;

        public UserRegistrationFunction(
            ILogger<UserRegistrationFunction> logger,
            IEmailService emailService,
            IRetryService retryService
        )
        {
            _logger = logger;
            _emailService = emailService;
            _retryService = retryService;
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
                    "Processing post-registration tasks for username: {Username}",
                    registrationMessage.Username
                );
                _logger.LogInformation("Request ID: {RequestId}", registrationMessage.RequestId);

                // Execute post-processing tasks
                await PerformPostRegistrationTasks(registrationMessage);

                _logger.LogInformation(
                    "Post-registration tasks completed for username: {Username}",
                    registrationMessage.Username
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Critical error processing user registration message: {MessageText}",
                    message.MessageText
                );
                // Only re-throw for critical errors (deserialization, etc..)
                // Post-processing task failures are handled individually and don't break the flow
                throw;
            }
        }

        private async Task PerformPostRegistrationTasks(UserRegistrationMessage message)
        {
            var taskResults = new List<(string TaskName, bool Success, string Error)>();

            _logger.LogInformation(
                "Starting post-registration tasks for user: {Username}",
                message.Username
            );

            // Execute all post-processing tasks with individual error handling
            var welcomeEmailResult = await ExecuteTaskSafely(
                () => SendWelcomeEmail(message),
                "Welcome Email"
            );
            taskResults.Add(welcomeEmailResult);

            var analyticsResult = await ExecuteTaskSafely(
                () => LogUserRegistrationAnalytics(message),
                "Analytics"
            );
            taskResults.Add(analyticsResult);

            var preferencesResult = await ExecuteTaskSafely(
                () => SetupDefaultPreferences(message),
                "User Preferences"
            );
            taskResults.Add(preferencesResult);

            // Log summary of all tasks
            var successfulTasks = taskResults.Count(r => r.Success);
            var failedTasks = taskResults.Count(r => !r.Success);

            _logger.LogInformation(
                "Post-registration tasks completed for user: {Username}. Successful: {SuccessfulTasks}, Failed: {FailedTasks}",
                message.Username,
                successfulTasks,
                failedTasks
            );

            // Log details of failed tasks
            foreach (var failedTask in taskResults.Where(r => !r.Success))
            {
                _logger.LogWarning(
                    "Task '{TaskName}' failed for user {Username}: {Error}",
                    failedTask.TaskName,
                    message.Username,
                    failedTask.Error
                );
            }

            // Don't throw exceptions - let the function complete successfully
            // Individual task failures don't break the main flow
        }

        private async Task<(string TaskName, bool Success, string Error)> ExecuteTaskSafely(
            Func<Task> task,
            string taskName
        )
        {
            try
            {
                await task();
                return (taskName, true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Task '{TaskName}' failed", taskName);
                return (taskName, false, ex.Message);
            }
        }

        private async Task SendWelcomeEmail(UserRegistrationMessage message)
        {
            try
            {
                _logger.LogInformation(
                    "Sending welcome email to: {Email} for username: {Username}",
                    message.Email,
                    message.Username
                );

                var emailSent = await _emailService.SendWelcomeEmailAsync(
                    message.Email,
                    message.Username
                );

                if (emailSent)
                {
                    _logger.LogInformation(
                        "Welcome email sent successfully to: {Email}",
                        message.Email
                    );
                }
                else
                {
                    _logger.LogWarning("Failed to send welcome email to: {Email}", message.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to: {Email}", message.Email);
                // Don't re-throw - continue with other post-processing tasks
            }
        }

        private async Task LogUserRegistrationAnalytics(UserRegistrationMessage message)
        {
            try
            {
                _logger.LogInformation(
                    "Logging analytics for new user registration: {Username}",
                    message.Username
                );

                // TODO: Integrate with analytics service
                // Example implementation structure:
                // var analyticsService = new AnalyticsService();
                // await analyticsService.TrackUserRegistrationAsync(message.Username, message.Email, message.RequestedAt);

                // Placeholder for actual analytics integration
                await Task.Delay(100); // Simulate async operation

                _logger.LogInformation(
                    "Analytics logged successfully for user: {Username}",
                    message.Username
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to log analytics for user: {Username}",
                    message.Username
                );
                // Don't re-throw - continue with other post-processing tasks
            }
        }

        private async Task SetupDefaultPreferences(UserRegistrationMessage message)
        {
            try
            {
                _logger.LogInformation(
                    "Setting up default user preferences for: {Username}",
                    message.Username
                );

                // TODO: Set up default user preferences, notification settings, etc.
                // Example implementation structure:
                // var preferencesService = new UserPreferencesService();
                // await preferencesService.SetupDefaultPreferencesAsync(message.Username);

                // Placeholder for actual preferences setup
                await Task.Delay(100); // Simulate async operation

                _logger.LogInformation(
                    "Default preferences set successfully for user: {Username}",
                    message.Username
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to set default preferences for user: {Username}",
                    message.Username
                );
                // Don't re-throw - continue with other post-processing tasks
            }
        }
    }
}
