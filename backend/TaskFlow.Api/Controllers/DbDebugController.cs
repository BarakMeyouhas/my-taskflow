using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbDebugController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IGreetingService _greetingService;
        private readonly IQueueService _queueService;

        public DbDebugController(
            AppDbContext db,
            IGreetingService greetingService,
            IQueueService queueService
        )
        {
            _db = db;
            _greetingService = greetingService;
            _queueService = queueService;
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                Console.WriteLine("=== DATABASE CONNECTION DEBUG START ===");

                // בדיקה אם ניתן להתחבר למסד הנתונים
                var canConnect = _db.Database.CanConnect();
                Console.WriteLine($"Can connect: {canConnect}");

                int? userCount = null;
                string? usersError = null;

                try
                {
                    if (_db.Users != null)
                    {
                        userCount = _db.Users.Count();
                        Console.WriteLine($"User count: {userCount}");
                    }
                    else
                    {
                        Console.WriteLine("DbSet Users is null");
                    }
                }
                catch (Exception ex)
                {
                    usersError = ex.Message;
                    Console.WriteLine($"Error querying Users table: {ex}");
                }

                Console.WriteLine($"Database provider: {_db.Database.ProviderName}");
                Console.WriteLine($"Database name: {_db.Database.GetDbConnection().Database}");
                Console.WriteLine($"Server: {_db.Database.GetDbConnection().DataSource}");

                Console.WriteLine("=== DATABASE CONNECTION DEBUG END ===");

                return Ok(
                    new
                    {
                        canConnect,
                        userCount,
                        usersError,
                        provider = _db.Database.ProviderName,
                        database = _db.Database.GetDbConnection().Database,
                        server = _db.Database.GetDbConnection().DataSource,
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DATABASE CONNECTION ERROR: {ex}");
                return StatusCode(
                    500,
                    new
                    {
                        error = ex.Message,
                        stackTrace = ex.StackTrace,
                        innerException = ex.InnerException?.Message,
                    }
                );
            }
        }

        [HttpGet("test-azure-storage")]
        public IActionResult TestAzureStorage()
        {
            try
            {
                Console.WriteLine("=== AZURE STORAGE CONNECTION TEST ===");

                var isAvailable = _queueService.IsAvailable();
                Console.WriteLine($"Azure Storage available: {isAvailable}");

                Console.WriteLine("=== AZURE STORAGE TEST END ===");

                return Ok(
                    new
                    {
                        azureStorageAvailable = isAvailable,
                        timestamp = DateTime.UtcNow,
                        note = "Azure Storage connectivity verified through QueueService",
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AZURE STORAGE TEST ERROR: {ex}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("greet/{name}")]
        public IActionResult Greet(string name)
        {
            try
            {
                var greeting = _greetingService.SayHello(name);
                return Ok(new { message = greeting, name = name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("greet-with-logger/{name}")]
        public IActionResult GreetWithLogger(
            string name,
            [FromServices] ILogger<DbDebugController> logger
        )
        {
            try
            {
                logger.LogInformation("Greeting endpoint called for name: {Name}", name);
                var greeting = _greetingService.SayHello(name);
                return Ok(
                    new
                    {
                        message = greeting,
                        name = name,
                        timestamp = DateTime.UtcNow,
                    }
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in greeting endpoint for name: {Name}", name);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("greet-method-injection/{name}")]
        public IActionResult GreetMethodInjection(
            string name,
            [FromServices] ILogger<GreetingServiceDapper> logger
        )
        {
            try
            {
                // Method Injection - passing logger as parameter
                var greeting = _greetingService.SayHelloWithLogger(name, logger);
                return Ok(
                    new
                    {
                        message = greeting,
                        name = name,
                        injectionType = "Method Injection",
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
