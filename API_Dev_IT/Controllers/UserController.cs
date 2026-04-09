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

namespace API_Dev_IT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IUser _user;
        private readonly IJwt _jwt;
        public UserController(BookingContext context, 
               ILogger<UserController> logger, 
               IUser user, IJwt jwt)
        {
            _context = context;
            _logger = logger;
            _user = user;
            _jwt = jwt;
        }

        [HttpGet("GetUser")]
        [Authorize(Roles = "Manager, Receptionist, Admin")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var user = await _context
                                 .User
                                 .AsNoTracking()
                                 .ToListAsync();
                return Ok(user);
            }
            catch(Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetUser/{id}")]
        [Authorize(Roles = "Manager, Receptionist, Admin")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = await _context
                                 .User
                                 .FirstAsync
                                 (x => x.UserID == id);
                return Ok(user);
            }
            catch(Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("UserLogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LogIn logIn)
        {
            try
            {
                var user = await _user.LogIn(logIn);
                var token = await _jwt.GenerateToken(user);

                return Ok(token);
            }
            catch(Exception x)
            {
                _logger.LogError($"{x.Message}");
                return Unauthorized(x.Message);
            }
        }

        [HttpPost("CreateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> Post(User users)
        {
            try
            {
                var insert = await _user.Create(users);
                var token = await _jwt.GenerateToken(insert);

                return Ok(token);

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize(Roles = "Admin , Customer")]
        public async Task<IActionResult> Put(User users, int id)
        {
            try
            {
                var user = await _user.Update(users, id);
                var token = await _jwt.GenerateToken(user);

                return Ok(token);

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("RemoveUser/{id}")]
        [Authorize(Roles = "Admin , Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var remove = await _user.Delete(id);
                return Ok(remove);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }

        }
    }
}
