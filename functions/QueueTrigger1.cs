using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TaskFlow.Functions.Models;

namespace TaskFlow.Functions;

// Commented out to prevent conflict with UserRegistrationFunction
// Both functions were listening to the same queue, causing only one to be triggered
/*
public class QueueTrigger1
{
    private readonly ILogger<QueueTrigger1> _logger;

    public QueueTrigger1(ILogger<QueueTrigger1> logger)
    {
        _logger = logger;
    }

    [Function(nameof(QueueTrigger1))]
    public void Run(
        [QueueTrigger("user-registration-queue", Connection = "AzureWebJobsStorage")]
            QueueMessage message
    )
    {
        _logger.LogInformation(
            "C# Queue trigger function processed message: {messageText}",
            message.MessageText
        );
        
        // This is a general queue trigger - you can use it for other purposes
        // The UserRegistrationFunction handles the actual user registration logic
    }
}
*/
