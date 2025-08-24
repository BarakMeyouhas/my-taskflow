using System.Data;
using Dapper;

namespace TaskFlow.Api.Services
{
    public interface IGreetingService
    {
        string SayHello(string name);
        string SayHelloWithLogger(string name, ILogger<GreetingServiceDapper> logger);
    }

    public class GreetingServiceDapper : IGreetingService
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<GreetingServiceDapper> _logger;

        public GreetingServiceDapper(
            IDbConnection dbConnection,
            ILogger<GreetingServiceDapper> logger
        )
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string SayHello(string name)
        {
            _logger.LogInformation("Greeting requested for name: {Name}", name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "hello anonymous";
            }

            // Example of using Dapper to query database (if you had a greetings table)
            try
            {
                // This is just an example - you could store greetings in a database
                var customGreeting = _dbConnection.QueryFirstOrDefault<string>(
                    "SELECT GreetingText FROM Greetings WHERE Name = @Name",
                    new { Name = name }
                );

                if (!string.IsNullOrEmpty(customGreeting))
                {
                    _logger.LogInformation(
                        "Found custom greeting for {Name}: {Greeting}",
                        name,
                        customGreeting
                    );
                    return customGreeting;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Could not retrieve custom greeting from database for {Name}",
                    name
                );
                // Fall back to default greeting
            }

            return $"hello {name}";
        }

        // Method Injection example
        public string SayHelloWithLogger(string name, ILogger<GreetingServiceDapper> logger)
        {
            logger.LogInformation("Greeting requested for name: {Name}", name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "hello anonymous";
            }

            return $"hello {name}";
        }
    }
}
