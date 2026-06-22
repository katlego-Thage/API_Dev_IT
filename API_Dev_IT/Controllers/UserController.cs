using API_Dev_IT.Context;
using API_Dev_IT.Helper;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;

namespace API_Dev_IT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly IUser _user;
        private readonly IJwt _jwt;
        private readonly ILogger<UserController> _logger;
        public UserController(BookingContext context, 
               IUser user, IJwt jwt, ILogger<UserController> logger)
        {
            _context = context;
            _user = user;
            _jwt = jwt;
            _logger = logger;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var user = await _context
                                 .User
                                 .AsNoTracking()
                                 .ToListAsync();
                _logger.LogInformation("All users retrieved successfully");
                return Ok(user);

            }
            catch (Exception x)
            {
                _logger.LogError(x, "Error occurred while retrieving all users");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetUser/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = await _context
                                 .User
                                 .FirstAsync
                                 (x => x.UserID == id);
                _logger.LogInformation("User {UserId} retrieved successfully", id);
                return Ok(user);
            }
            catch(Exception x)
            {               
                _logger.LogError(x, "Error occurred while retrieving user {UserId}", id);
                return BadRequest(x.Message);
            }
        }

        [HttpPost("UserLogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LogIn logIn)
        {
            try
            {
                var user = await _user.LogIn<User>(logIn);
                var token = await _jwt.GenerateToken(user);
                _logger.LogInformation("User {UserId} logged in successfully", user.UserID);
                return Ok(token);
            }
            catch(Exception x)
            {
                _logger.LogError(x, "Error occurred while logging in user");
                return Unauthorized(x.Message);
            }
        }

        [HttpPost("CreateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> Post(User users)
        {
            try
            {  
                var insert = await _user.Create<User>(users);
                var token = await _jwt.GenerateToken(insert);
                _logger.LogInformation("User {UserId} created successfully", insert.UserID);
                return Ok(token);

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, "Error occurred while creating user");  
                return BadRequest(x.Message);
            }
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize(Roles = "Admin , Customer")]
        public async Task<IActionResult> Put(User users, int id)
        {
            try
            {
                var user = await _user.Update<User>(users, id);
                var token = await _jwt.GenerateToken(user);
                _logger.LogInformation("User {UserId} updated successfully", id);
                return Ok(token);

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, "Error occurred while updating user {UserId}", id);
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("RemoveUser/{id}")]
        [Authorize(Roles = "Admin , Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var remove = await _user.Delete<User>(id);
                _logger.LogInformation("User {UserId} removed successfully", id);
                return Ok(remove);
            }
            catch (Exception x)
            {
                _logger.LogError(x, "Error occurred while removing user {UserId}", id);
                return BadRequest(x.Message);
            }
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Refresh token validation failed");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Token refresh attempt");
                var principal = _jwt.ValidateToken(request.AccessToken);

                if (principal == null)
                {
                    _logger.LogWarning("Token refresh failed: invalid access token");
                    return Unauthorized(new { Message = "Invalid token" });
                }

                var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                {
                    _logger.LogWarning("Token refresh failed: invalid token claims");
                    return Unauthorized(new { Message = "Invalid token claims" });
                }

                var user = await _context.User
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                {
                    _logger.LogWarning("Token refresh failed: user {UserId} not found", userId);
                    return Unauthorized(new { Message = "User not found" });
                }

                var newToken = await _jwt.GenerateToken(user);

                _logger.LogInformation(
                    "Token refreshed successfully for user {UserId}",
                    userId);

                return Ok(newToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh error");
                return StatusCode(500, new { Message = "An error occurred during token refresh" });
            }
        }
    }
}
