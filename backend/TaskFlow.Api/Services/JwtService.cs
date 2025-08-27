using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskFlow.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username);
        bool ValidateToken(string token);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Get JWT settings from configuration
            _jwtKey =
                Environment.GetEnvironmentVariable("JWT_KEY") ?? configuration["JwtSettings:Key"]!;

            // Ensure JWT key is configured
            if (string.IsNullOrEmpty(_jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT_KEY is not configured. Please set JWT_KEY environment variable or JwtSettings:Key in configuration."
                );
            }

            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "taskflow";
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "taskflow";

            // Debug logging
            _logger.LogInformation("=== JWT SERVICE DEBUG ===");
            _logger.LogInformation(
                "Environment JWT_KEY: {JwtKey}",
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_KEY"))
                    ? "NOT SET"
                    : Environment
                        .GetEnvironmentVariable("JWT_KEY")!
                        .Substring(
                            0,
                            Math.Min(10, Environment.GetEnvironmentVariable("JWT_KEY")!.Length)
                        ) + "..."
            );
            _logger.LogInformation(
                "Config JwtSettings:Key: {ConfigKey}",
                string.IsNullOrEmpty(configuration["JwtSettings:Key"])
                    ? "NOT SET"
                    : configuration["JwtSettings:Key"]!.Substring(
                        0,
                        Math.Min(10, configuration["JwtSettings:Key"]!.Length)
                    ) + "..."
            );
            _logger.LogInformation("Final JWT Key Length: {KeyLength}", _jwtKey?.Length ?? 0);
            _logger.LogInformation("=== END JWT DEBUG ===");
        }

        public string GenerateToken(string username)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[] { new Claim(ClaimTypes.Name, username) };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

                _logger.LogInformation(
                    "JWT token generated successfully for user: {Username}",
                    username
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user: {Username}", username);
                throw;
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtKey);

                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _issuer,
                        ValidateAudience = true,
                        ValidAudience = _audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed");
                return false;
            }
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtKey);

                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _issuer,
                        ValidateAudience = true,
                        ValidAudience = _audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken
                );

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting claims from JWT token");
                throw;
            }
        }
    }
}
