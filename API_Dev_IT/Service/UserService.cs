using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_Dev_IT.Service
{
    public class UserService : IUser
    {
            private readonly BookingContext _context;
            private readonly ILogger<UserService> _logger;
            private readonly IPasswordHasher<User> _passwordHasher;

            public UserService(
                BookingContext context,
                ILogger<UserService> logger)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _passwordHasher = new PasswordHasher<User>();
            }

            public async Task<T> LogIn<T>(LogIn login)
            {
                try
                {
                    if (login == null)
                    {
                        _logger.LogWarning("Login failed: request is null");
                        throw new ArgumentNullException(nameof(login));
                    }

                        _logger.LogInformation(
                        $"Login attempt for user {login.email}");

                    var user = await _context.User
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Email != null &&
                        x.Email.ToLower() == login.email.ToLower());

                    if (user == null)
                    {
                        _logger.LogWarning(
                        $"Login failed: user not found for email {login.email}");

                        throw new UnauthorizedAccessException("Invalid email or password");
                    }

                    var result = _passwordHasher.VerifyHashedPassword(
                        user,
                        user.PasswordHash ?? string.Empty,
                        login.password ?? string.Empty);
                    if (result == PasswordVerificationResult.Failed)
                    {
                        _logger.LogWarning(
                            $"Login failed: invalid password for user {user.UserID}");
                        throw new UnauthorizedAccessException("Invalid email or password");
                    }

                    if (result == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        _logger.LogInformation(
                        $"Password rehash needed for user {user.UserID}");
                    }

                    _logger.LogInformation(
                    $"User {user.UserID} logged in successfully with role {user.RoleID}");

                    if (typeof(T) == typeof(User))
                    {
                        return (T)(object)user;
                    }

                    throw new InvalidCastException($"Cannot convert User to {typeof(T).Name}");
                }
                catch (Exception ex)
                { 
                    _logger.LogError(ex, "An error occurred during login");
                    throw;
                }
            }

            public async Task<T> Create<T>(User register)
            {
                try
                {
                    if (register == null)
                    {
                        _logger.LogWarning("Registration failed: request is null");
                        throw new ArgumentNullException(nameof(register));
                    }

                    _logger.LogInformation(
                        $"Registration attempt for email {register.Email} with " +
                        $"username {register.Username}");

                    var existingUser = await _context.User
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Email != null &&
                            x.Email.ToLower() == register.Email.ToLower());

                    if (existingUser != null)
                    {
                        _logger.LogWarning(
                            $"Registration failed: email {register.Email} already exists");
                        throw new InvalidOperationException($"An account with this email " +
                                                            $"{register.Email} already exists");
                    }

                    var role = await _context.role.FindAsync(register.RoleID);
                    if (role == null)
                    {
                        _logger.LogWarning(
                            $"Registration failed: role {register.RoleID} does not exist");
                        throw new InvalidOperationException("Invalid role selected");
                    }

                    var passwordHash = _passwordHasher.HashPassword(
                                       null!, register.PasswordHash ?? string.Empty);

                    var insert = new User
                    {
                        Username = register.Username?.Trim() ?? string.Empty,
                        PasswordHash = passwordHash,
                        Email = register.Email?.ToLower().Trim() ?? string.Empty,
                        RoleID = register.RoleID,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.User.Add(insert);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        $"User {insert.UserID} created successfully with role {register.RoleID}");

                    if (typeof(T) == typeof(User))
                    {
                        return (T)(object)insert;
                    }

                    throw new InvalidCastException($"Cannot convert User" +
                                                   $" to {typeof(T).Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during user registration");
                    throw;
                }
            }

            public async Task<T> Update<T>(User users, int id)
            {
                try
                {
                    if (users == null)
                    {
                        _logger.LogWarning($"Update failed: request is null " +
                                           "for user {UserId}", id);
                        throw new ArgumentNullException(nameof(users));
                    }

                    _logger.LogInformation(
                              $"Updating user {id}. Fields provided: Username=" +
                              $"{!string.IsNullOrWhiteSpace(users.Username)}, " +
                              $"Email={!string.IsNullOrWhiteSpace(users.Email)}, " +
                              $"RoleId={users.RoleID.HasValue}");

                    var user = await _context.User.FirstOrDefaultAsync(x => x.UserID == id);

                    if (user == null)
                    {
                        _logger.LogWarning($"Update failed: user {id} not found");
                        throw new InvalidOperationException("User not found");
                    }

                    if (!string.IsNullOrWhiteSpace(users.Username))
                    {
                        user.Username = users.Username.Trim();
                        _logger.LogDebug($"Updated username for user {id}");
                    }

                    if (!string.IsNullOrWhiteSpace(users.Email))
                    {
                        var emailExists = await _context.User
                            .AsNoTracking()
                            .AnyAsync(x => x.Email != null &&
                                x.Email.ToLower() == users.Email.ToLower() &&
                                x.UserID != id);

                        if (emailExists)
                        {
                            _logger.LogWarning($"Update failed: email {users.Email} is already in use");
                            throw new InvalidOperationException("Email is already in use");
                        }

                        user.Email = users.Email.ToLower().Trim();

                        _logger.LogDebug($"Updated email for user {id}");
                    }

                    if (users.RoleID.HasValue)
                    {
                        var role = await _context.role.FindAsync(users.RoleID.Value);
                        if (role == null)
                        {
                            _logger.LogWarning(
                                $"Update failed: role {users.RoleID.Value} does not exist");
                            throw new InvalidOperationException("Invalid role");
                        }

                        user.RoleID = users.RoleID.Value;

                        _logger.LogDebug($"Updated role for user {id} to {users.RoleID.Value}");
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"User {id} updated successfully");
                    return (T)(object)user;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating user {id}");
                    throw;
                }
                
            }

        public async Task<T> Delete<T>(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete user {id}");

                var user = await _context.User
                    .FirstOrDefaultAsync(x => x.UserID == id);

                if (user == null)
                {
                    _logger.LogWarning($"Delete failed: user {id} not found");
                    throw new InvalidOperationException("User not found");
                }

                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {id} deleted successfully");
                return (T)(object)user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting user {id}");
                throw;
            }
        }
    }
}
