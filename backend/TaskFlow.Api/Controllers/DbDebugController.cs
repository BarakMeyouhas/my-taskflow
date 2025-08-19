using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbDebugController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DbDebugController(AppDbContext db)
        {
            _db = db;
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
                string usersError = null;

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
    }
}
