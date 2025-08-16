using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
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
                var userCount = db.Users.Count();
                return Ok(
                    new
                    {
                        message = "Database connection working!",
                        userCount = userCount,
                        timestamp = DateTime.UtcNow,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        error = "Database error",
                        details = ex.Message,
                        stackTrace = ex.StackTrace,
                    }
                );
            }
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
            [FromBody] UserRegisterRequest request,
            [FromServices] AppDbContext db
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

                // Check if user already exists
                if (db.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
                {
                    return BadRequest("Username or email already exists");
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user
                var newUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                };

                // Add to database
                db.Users.Add(newUser);
                db.SaveChanges();

                _logger.LogInformation(
                    "User registered successfully for username: {Username}",
                    request.Username
                );

                return Ok(new { message = "User registered successfully", userId = newUser.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error registering user for username: {Username}",
                    request.Username
                );
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
