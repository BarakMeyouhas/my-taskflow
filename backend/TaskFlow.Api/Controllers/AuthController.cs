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
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IQueueService _queueService;

        public AuthController(
            ILogger<AuthController> logger,
            IUserService userService,
            IJwtService jwtService,
            IQueueService queueService
        )
        {
            _logger = logger;
            _userService = userService;
            _jwtService = jwtService;
            _queueService = queueService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", request?.Username);

                // Validate request
                if (
                    request == null
                    || string.IsNullOrEmpty(request.Username)
                    || string.IsNullOrEmpty(request.Password)
                )
                {
                    _logger.LogWarning("Login failed: Invalid request data");
                    return BadRequest("Username and password are required");
                }

                // Authenticate user using service
                var user = await _userService.AuthenticateUserAsync(
                    request.Username,
                    request.Password
                );

                if (user == null)
                {
                    _logger.LogWarning(
                        "Login failed: Invalid credentials for username: {Username}",
                        request.Username
                    );
                    return Unauthorized("Invalid username or password");
                }

                // Generate JWT token using service
                var token = _jwtService.GenerateToken(user.Username);

                _logger.LogInformation(
                    "Login successful for username: {Username}",
                    request.Username
                );

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
                _logger.LogError(
                    ex,
                    "Error during login for username: {Username}",
                    request?.Username
                );
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
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                _logger.LogInformation("Testing database connection...");

                // Test user service to verify database connectivity
                var testUser = await _userService.GetUserByUsernameAsync("test");

                return Ok(
                    new
                    {
                        message = "Database connection working!",
                        timestamp = DateTime.UtcNow,
                        note = "Database connectivity verified through UserService",
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed");
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
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
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
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
                if (await _userService.UserExistsAsync(request.Username, request.Email))
                {
                    return BadRequest("Username or email already exists");
                }

                // Check if queue service is available
                if (!_queueService.IsAvailable())
                {
                    _logger.LogWarning(
                        "Queue service is not available. Cannot process user registration."
                    );
                    return StatusCode(
                        503,
                        new
                        {
                            error = "Service temporarily unavailable",
                            details = "User registration service is currently unavailable. Please try again later.",
                            code = "QUEUE_SERVICE_UNAVAILABLE",
                        }
                    );
                }

                // Create user using service
                var newUser = await _userService.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.Password
                );

                // Send registration message to queue
                var messageSent = await _queueService.SendUserRegistrationMessageAsync(
                    newUser,
                    request.Password
                );

                if (!messageSent)
                {
                    _logger.LogError(
                        "Failed to send user registration message to queue for username: {Username}",
                        request.Username
                    );
                    return StatusCode(
                        500,
                        new { error = "Failed to process registration request" }
                    );
                }

                _logger.LogInformation(
                    "User registration request queued successfully for username: {Username}",
                    request.Username
                );

                return Ok(
                    new
                    {
                        message = "User registration request received and queued successfully",
                        requestId = Guid.NewGuid().ToString(),
                        status = "queued",
                        username = request.Username,
                        email = request.Email,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing user registration request for username: {Username}",
                    request.Username
                );
                return StatusCode(
                    500,
                    new { error = "Internal server error", details = ex.Message }
                );
            }
        }
    }

    public class UserLoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserRegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
