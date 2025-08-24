// namespace TaskFlow.Api.Services
// {
//     public interface IGreetingService
//     {
//         string SayHello(string name);
//         string SayHelloWithLogger(string name, ILogger<GreetingService> logger);
//     }

//     public class GreetingService : IGreetingService
//     {
//         private readonly ILogger<GreetingService> _logger;

//         public GreetingService(ILogger<GreetingService> logger)
//         {
//             _logger = logger;
//         }

//         public string SayHello(string name)
//         {
//             _logger.LogInformation("Greeting requested for name: {Name}", name);

//             if (string.IsNullOrWhiteSpace(name))
//             {
//                 return "hello anonymous";
//             }

//             return $"hello {name}";
//         }

//         // Method Injection example
//         public string SayHelloWithLogger(string name, ILogger<GreetingService> logger)
//         {
//             logger.LogInformation("Greeting requested for name: {Name}", name);

//             if (string.IsNullOrWhiteSpace(name))
//             {
//                 return "hello anonymous";
//             }

//             return $"hello {name}";
//         }
//     }
// }
