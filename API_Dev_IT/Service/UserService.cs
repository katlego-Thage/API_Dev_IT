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

        public UserService(BookingContext context, 
               ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<User> LogIn(LogIn logIn)
        {
            var user = await _context.User
                             .FirstAsync(x => 
                             x.Email == logIn.email);

            if (user is null)
            {
                _logger.LogError($"{user} Invalid User");
                throw new UnauthorizedAccessException(
                          "Invalid User");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(
                         user,
                         user.PasswordHash ?? string.Empty,
                         logIn.password ??string.Empty);

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogError($"{user.PasswordHash} " +
                          $"Invalid credentials");
                throw new UnauthorizedAccessException(
                          "Invalid credentials");
            }
            return user;
        }
        public async Task<User> Create(User user)
        {
            var userAdd = await _context.User
                         .FirstOrDefaultAsync( 
                          x => x.Email == user.Email);

            if (user?.Email is null || userAdd?.Email == user?.Email)
            {
                _logger.LogError($"user {userAdd} already exist");
                throw new InvalidOperationException(
                          "User already exist");
            }

            var role = await _context.role.FindAsync(user?.RoleID.Value);

            if (role == null)
            {
                _logger.LogError($"role {role?.RoleID} does not exist");
                throw new InvalidOperationException($"role " +
                          $"{role?.RoleID} does not exist");
            }

            var passwordHasher = new PasswordHasher<User>();
            var insert = new User
            {
                Username = user?.Username,
                PasswordHash = user.PasswordHash = passwordHasher.HashPassword
                              (user,
                               user?.PasswordHash??string.Empty
                               ),
                Email = user?.Email,
                RoleID = role?.RoleID,
                CreatedAt = user?.CreatedAt,
            };

            _context.Add(insert);
            await _context.SaveChangesAsync();


            return insert;
        }

        public async Task<User> Update(User users, int id)
        {
            var user = await _context.User
                      .FirstOrDefaultAsync(x => x.UserID == id);

            if (user is null && user?.UserID != users.UserID)
            {
                _logger.LogError($"{user} Invalid exist");
                throw new InvalidOperationException("Invalid User");
            }

            user.Username = users.Username;
            user.Email = users.Email;
            user.RoleID = users.RoleID;
            user.PasswordHash = user.PasswordHash;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return (user);
        }

        public async Task<User> Delete(int id)
        {
            var user = await _context.User
                       .FirstOrDefaultAsync(x => x.UserID == id);

            if (user?.UserID is null || user.UserID != id)
            {
                _logger.LogError($"{user} Invalid exist");
                throw new InvalidOperationException("Invalid user");
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
