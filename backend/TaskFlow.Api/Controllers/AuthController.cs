using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        // Temporarily disabled queue service for startup testing
        // private readonly IQueueService _queueService;

        public AuthController(
            ILogger<AuthController> logger
        // Temporarily disabled queue service for startup testing
        // IQueueService queueService
        )
        {
            _logger = logger;
            // _queueService = queueService;
        }

        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] UserLoginRequest request,
            [FromServices] AppDbContext db
        )
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"Login attempt for username: {request?.Username}");

                // Validate request
                if (
                    request == null
                    || string.IsNullOrEmpty(request.Username)
                    || string.IsNullOrEmpty(request.Password)
                )
                {
                    Console.WriteLine("Login failed: Invalid request data");
                    return BadRequest("Username and password are required");
                }

                Console.WriteLine("Attempting to query database...");

                // Find user in database
                var user = db.Users.FirstOrDefault(u => u.Username == request.Username);

                Console.WriteLine($"User found: {user != null}");

                if (user == null)
                {
                    Console.WriteLine("Login failed: User not found");
                    return Unauthorized("Invalid username or password");
                }

                Console.WriteLine("Verifying password...");

                // Verify password hash
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    Console.WriteLine("Login failed: Invalid password");
                    return Unauthorized("Invalid username or password");
                }

                Console.WriteLine("Generating JWT token...");

                // Generate token and return user info
                var token = GenerateJwtToken(user.Username);

                Console.WriteLine("Login successful");

                return Ok(
                    new
                    {
                        token,
                        user = new
                        {
                            id = user.Id,
                            username = user.Username,
                            email = user.Email,
                        },
                    }
                );
            }
            catch (Exception ex)
            {
                // Log the exception with full details
                Console.WriteLine($"=== LOGIN ERROR ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                Console.WriteLine($"=== END LOGIN ERROR ===");

                return StatusCode(
                    500,
                    new { error = "Internal server error", details = ex.Message }
                );
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Auth controller is working!", timestamp = DateTime.UtcNow });
        }

        [HttpGet("db-test")]
        public IActionResult TestDatabase([FromServices] AppDbContext db)
        {
            try
            {
                // Log connection attempt details
                Console.WriteLine($"=== DATABASE CONNECTION TEST ===");
                Console.WriteLine($"Attempting to connect to database...");
                Console.WriteLine($"Database provider: {db.Database.ProviderName}");
                Console.WriteLine($"Database name: {db.Database.GetDbConnection().Database}");
                Console.WriteLine($"Server: {db.Database.GetDbConnection().DataSource}");

                var userCount = db.Users.Count();

                Console.WriteLine($"Connection successful! User count: {userCount}");
                Console.WriteLine($"=== END DATABASE TEST ===");

                return Ok(
                    new
                    {
                        message = "Database connection working!",
                        userCount = userCount,
                        timestamp = DateTime.UtcNow,
                        connectionInfo = new
                        {
                            provider = db.Database.ProviderName,
                            database = db.Database.GetDbConnection().Database,
                            server = db.Database.GetDbConnection().DataSource,
                        },
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== DATABASE CONNECTION ERROR ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                Console.WriteLine($"=== END DATABASE ERROR ===");

                // Enhanced error handling to show inner exception details
                var errorDetails = new
                {
                    error = "Database error",
                    details = ex.Message,
                    exceptionType = ex.GetType().Name,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException != null
                        ? new
                        {
                            message = ex.InnerException.Message,
                            type = ex.InnerException.GetType().Name,
                            stackTrace = ex.InnerException.StackTrace,
                        }
                        : null,
                    // Show all inner exceptions in the chain
                    allInnerExceptions = GetAllInnerExceptions(ex),
                };

                return StatusCode(500, errorDetails);
            }
        }

        private List<object> GetAllInnerExceptions(Exception ex)
        {
            var innerExceptions = new List<object>();
            var current = ex.InnerException;
            var depth = 0;

            while (current != null && depth < 5) // Limit to 5 levels to avoid infinite loops
            {
                innerExceptions.Add(
                    new
                    {
                        depth = depth,
                        type = current.GetType().Name,
                        message = current.Message,
                        stackTrace = current.StackTrace,
                    }
                );

                current = current.InnerException;
                depth++;
            }

            return innerExceptions;
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345")
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Name, username) };

            var token = new JwtSecurityToken(
                issuer: "taskflow",
                audience: "taskflow",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("register")]
        public IActionResult Register(
            [FromBody] UserRegisterRequest request
        // Temporarily disabled database dependency for startup testing
        // [FromServices] AppDbContext db
        )
        {
            try
            {
                // Validate request
                if (
                    string.IsNullOrEmpty(request.Username)
                    || string.IsNullOrEmpty(request.Email)
                    || string.IsNullOrEmpty(request.Password)
                )
                {
                    return BadRequest("Username, email, and password are required");
                }

                // Temporarily disabled database operations for startup testing
                // // Check if user already exists
                // if (db.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
                // {
                //     return BadRequest("Username or email already exists");
                // }

                // Check if queue service is available
                // if (!_queueService.IsAvailable())
                // {
                //     _logger.LogWarning(
                //         "Queue service is not available. Cannot process user registration."
                //     );
                //     return StatusCode(
                //         503,
                //         new
                //         {
                //             error = "Service temporarily unavailable",
                //             details = "User registration service is currently unavailable. Please try again later.",
                //             code = "QUEUE_SERVICE_UNAVAILABLE",
                //         }
                //     );
                // }

                // Create user object (without password hash - will be done by the function)
                var newUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = string.Empty, // Will be set by the function
                };

                // Send registration message to queue
                // var messageSent = await _queueService.SendUserRegistrationMessageAsync(
                //     newUser,
                //     request.Password
                // );

                // if (!messageSent)
                // {
                //     _logger.LogError(
                //         "Failed to send user registration message to queue for username: {Username}",
                //         request.Username
                //     );
                //     return StatusCode(
                //         500,
                //         new { error = "Failed to process registration request" }
                //     );
                // }

                // _logger.LogInformation(
                //     "User registration request queued successfully for username: {Username}",
                //     request.Username
                // );

                return Ok(
                    new
                    {
                        message = "User registration request received (TEST MODE - no actual processing)",
                        requestId = Guid.NewGuid().ToString(),
                        status = "test_mode",
                        note = "Database and Queue services temporarily disabled for startup testing. This is a simulation only.",
                        username = request.Username,
                        email = request.Email,
                    }
                );
            }
            catch (Exception ex)
            {
                // _logger.LogError(
                //     ex,
                //     "Error processing user registration request for username: {Username}",
                //     request.Username
                // );
                return StatusCode(
                    500,
                    new { error = "Internal server error", details = ex.Message }
                );
            }
        }

        public class UserRegisterRequest
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }

    public class UserLoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserRegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
