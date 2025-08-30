using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TaskFlow.Functions.Services
{
    public interface IEmailService
    {
        Task<bool> SendWelcomeEmailAsync(string email, string username);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly ILogger<EmailService> _logger;
        private readonly IRetryService _retryService;
        private readonly string _mailFromAddress;
        private readonly string _domain;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IRetryService retryService)
        {
            var connectionString = configuration["AzureCommunicationServicesConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Communication Services connection string is not configured"
                );
            }

            _emailClient = new EmailClient(connectionString);
            _logger = logger;
            _retryService = retryService;
            _mailFromAddress = "DoNotReply@4b34d6ec-157a-4d30-8277-bdfb85f57fc8.azurecomm.net";
            _domain = "4b34d6ec-157a-4d30-8277-bdfb85f57fc8.azurecomm.net";
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string username)
        {
            try
            {
                var subject = "Welcome to TaskFlow! 🎉";
                var htmlContent = GenerateWelcomeEmailHtml(username);

                var emailMessage = new EmailMessage(
                    senderAddress: _mailFromAddress,
                    recipients: new EmailRecipients([new EmailAddress(email)]),
                    content: new EmailContent(subject) { Html = htmlContent }
                );

                // Use retry logic for email sending
                var response = await _retryService.ExecuteWithRetryAsync(async () =>
                {
                    return await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);
                }, "Send welcome email", maxRetries: 3, delayMs: 2000);

                if (response.Value.Status == EmailSendStatus.Succeeded)
                {
                    _logger.LogInformation(
                        "Welcome email sent successfully to {Email} for user {Username}",
                        email,
                        username
                    );
                    return true;
                }
                else
                {
                    _logger.LogWarning(
                        "Welcome email failed to send to {Email} for user {Username}. Status: {Status}",
                        email,
                        username,
                        response.Value.Status
                    );
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending welcome email to {Email} for user {Username}",
                    email,
                    username
                );
                return false;
            }
        }

        private string GenerateWelcomeEmailHtml(string username)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to TaskFlow</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f8f9fa;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 32px;
            font-weight: bold;
            color: #007bff;
            margin-bottom: 10px;
        }}
        .welcome-text {{
            font-size: 24px;
            color: #28a745;
            margin-bottom: 20px;
        }}
        .content {{
            margin-bottom: 25px;
        }}
        .feature-list {{
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }}
        .feature-item {{
            margin: 10px 0;
            padding-left: 20px;
            position: relative;
        }}
        .feature-item:before {{
            content: '✓';
            color: #28a745;
            font-weight: bold;
            position: absolute;
            left: 0;
        }}
        .cta-button {{
            display: inline-block;
            background-color: #007bff;
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #dee2e6;
            color: #6c757d;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""logo"">📋 TaskFlow</div>
            <div class=""welcome-text"">Welcome aboard, {username}!</div>
        </div>
        
        <div class=""content"">
            <p>Thank you for joining TaskFlow! We're excited to help you organize your tasks, manage projects, and boost your productivity.</p>
            
            <div class=""feature-list"">
                <h3>🚀 What you can do with TaskFlow:</h3>
                <div class=""feature-item"">Create and organize tasks with intuitive drag-and-drop</div>
                <div class=""feature-item"">Track project progress with real-time updates</div>
                <div class=""feature-item"">Collaborate with team members seamlessly</div>
                <div class=""feature-item"">Generate insightful reports and analytics</div>
                <div class=""feature-item"">Access your workspace from anywhere, anytime</div>
            </div>
            
            <p>Ready to get started? Click the button below to access your dashboard:</p>
            
            <a href=""#"" class=""cta-button"">🚀 Launch TaskFlow</a>
            
            <p><strong>Pro tip:</strong> Take a few minutes to explore the interface and set up your first project. You'll be amazed at how easy it is to get organized!</p>
        </div>
        
        <div class=""footer"">
            <p>This email was sent from TaskFlow. If you have any questions, please don't hesitate to reach out to our support team.</p>
            <p>&copy; 2024 TaskFlow. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
