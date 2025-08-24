using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateUserAsync(string username, string password);
        Task<bool> UserExistsAsync(string username, string email);
        Task<User> CreateUserAsync(string username, string email, string password);
        Task<User?> GetUserByUsernameAsync(string username);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    _logger.LogWarning(
                        "Authentication failed: User not found - {Username}",
                        username
                    );
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning(
                        "Authentication failed: Invalid password for user - {Username}",
                        username
                    );
                    return null;
                }

                _logger.LogInformation("User authenticated successfully: {Username}", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error during user authentication for username: {Username}",
                    username
                );
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            try
            {
                return await _dbContext.Users.AnyAsync(u =>
                    u.Username == username || u.Email == email
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error checking if user exists - Username: {Username}, Email: {Email}",
                    username,
                    email
                );
                throw;
            }
        }

        public async Task<User> CreateUserAsync(string username, string email, string password)
        {
            try
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {Username}", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", username);
                throw;
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }
    }
}
