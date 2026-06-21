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
        private readonly IRoom _room;
        private readonly ILogger<RoomController> _logger;
        public RoomController(BookingContext context,
               IRoom room, ILogger<RoomController> logger)
        {
            _context = context;
            _room = room;
            _logger = logger;
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
                _logger.LogInformation($"All rooms retrieved successfully");
                return Ok(room);
            }
            catch(Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving all rooms");
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
                _logger.LogInformation($"Room {id} retrieved successfully");
                return Ok(room);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving room {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("Rom")]
        [Authorize(Roles = "Admin , Manager")]

        public async Task<IActionResult> Post(Room room)
        {
            try
            {
                var rooms = await _room.Create<Room>(room);
                _logger.LogInformation($"Room created successfully with ID {rooms.RoomID}");
                return Ok(rooms);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, $"Error occurred while creating room");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("Tenant/{id}")]
        [Authorize(Roles = "Admin , Manager")]
        public async Task<IActionResult> Put(Room room, int id)
        {
            try
            {
                var rooms = await _room.Update<Room>(room, id);
                _logger.LogInformation($"Room {id} updated successfully");
                return Ok(rooms);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, $"Error occurred while updating room {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin , Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var rooms = await _room.Delete<Room>(id);
                _logger.LogInformation($"Room {id} deleted successfully");
                return Ok(rooms);

            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while deleting room {id}");
                return BadRequest(x.Message);
            }
        }
    }
}
