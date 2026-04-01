using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                throw new UnauthorizedAccessException(
                          "Invalid credentials");
            }

            return user;
        }
    }
}
