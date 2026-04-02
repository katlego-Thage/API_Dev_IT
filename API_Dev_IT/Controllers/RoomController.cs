using API_Dev_IT.Context;
using API_Dev_IT.Helper;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IRoom _room;
        public RoomController(BookingContext context,
               ILogger<UserController> logger,
               IRoom room)
        {
            _context = context;
            _logger = logger;
            _room = room;
        }

        [HttpGet("GetRoom")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var room = await _context.room
                          .AsNoTracking()
                          .ToListAsync();
                return Ok(room);
            }
            catch(Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }          
        }

        [HttpGet("GetRoom/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {            
            try
            {
                var room = await _context.room
                         .FirstOrDefaultAsync(x => 
                         x.RoomID == id);
                return Ok(room);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("Rom")]
        [Authorize(Roles = "Admin , Manager")]

        public async Task<IActionResult> Post(Room room)
        {
            try
            {
                var userRole = UserRoleHelper.GetRole();
                if(userRole is not "Manager" || userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized add rooms");
                }
                var rooms = await _room.Create(room);
                return Ok(rooms);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("Tenant/{id}")]
        [Authorize(Roles = "Admin , Manager")]
        public async Task<IActionResult> Put(Room room, int id)
        {
            try
            {
                var userRole = UserRoleHelper.GetRole();
                if (userRole is not "Manager" || userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized add rooms");
                }
                var rooms = await _room.Update(room, id);
                return Ok(rooms);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin , Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userRole = UserRoleHelper.GetRole();
                if (userRole is not "Manager" || userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized add rooms");
                }
                var rooms = await _room.Delete(id);
                return Ok(rooms);

            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }
    }
}
