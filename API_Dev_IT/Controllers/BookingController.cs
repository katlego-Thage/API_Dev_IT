using API_Dev_IT.Context;
using API_Dev_IT.Helper;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly IPayment _payment;
        private readonly IBooking _booking;
        private readonly UserRoleHelper _helper;
        private readonly ILogger<BookingController> _logger;
        public BookingController(BookingContext context,
               IPayment payment, 
               IBooking booking,
               UserRoleHelper helper,
               ILogger<BookingController> logger)
        {
            _context = context;
            _payment = payment;
            _booking = booking;
            _helper = helper;
            _logger = logger;
        }

        [HttpGet("GetBooking")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userRole = User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .Role)?
                                   .Value;

                var userId = int.Parse(User.FindFirst(System
                                    .Security
                                    .Claims
                                    .ClaimTypes
                                    .NameIdentifier)?
                                    .Value ?? "0");

                if (userRole == "Customer")
                {
                    var myBookings = await _context.booking
                                    .Where(b => b.UserId == userId)
                                    .AsNoTracking()
                                    .ToListAsync();

                    return Ok(myBookings);
                }

                var allBookings = await _context.booking
                                    .AsNoTracking()
                                    .ToListAsync();
                _logger.LogInformation($"All bookings retrieved successfully");
                return Ok(allBookings);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving bookings");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetBooking/{id}")]
        [Authorize]
        public async Task<IActionResult> GetId(int id)
        {
            try
            {
                var userRole = User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .Role)?
                                   .Value;

                var userId = int.Parse(User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .NameIdentifier)?
                                   .Value ?? "0");

                var booking = await _context.booking
                                   .FirstAsync(x => x.BookingID == id);

                if (string.Equals(userRole, "Customer") && booking.UserId != userId)
                {
                    return Forbid();
                }
                _logger.LogInformation($"Booking {id} retrieved successfully");
                return Ok(booking);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving booking {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("AvailableRooms")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableRooms( 
                       [FromQuery] DateTime? checkIn,
                       [FromQuery] DateTime? checkOut,
                       [FromQuery] int? roomType)
        {
            try
            {
                var query = _context.room
                    .Where(r => r.IsAvailable == true)
                    .AsQueryable();

                if (roomType != null)
                {
                    query = query.Where(r => r.RoomID == roomType);
                }

                var availableRooms = await query.ToListAsync();
                _logger.LogInformation($"Available rooms retrieved successfully");
                return Ok(availableRooms);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving available rooms");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetBookingDetails/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            try
            { 
                var bookingDetails = await _helper.BookingDetails(id);

                if (bookingDetails is null)
                {
                    return NotFound();
                }

                var booking = await _context.booking.FindAsync(id);

                if (booking?.UserId != id)
                {
                    return Forbid();
                }

                _logger.LogInformation($"Booking details for {id} retrieved successfully");
                return Ok(bookingDetails);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving booking details for {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            try
            {
                var room = await _context.room.FindAsync(booking.RoomID);

                if (room == null || room.IsAvailable == false)
                {
                    return BadRequest("Room is not available");
                }

                booking.BookingDate = DateTime.Now;
                booking.Status = "Pending";

                var createdBooking = await _booking.Create<Booking>(booking);

                _logger.LogInformation($"Booking {createdBooking.BookingID} created successfully");
                return Ok(createdBooking);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, $"Error occurred while creating booking");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("UpdateBooking/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking booking)
        {
            try
            {
                var userRole = User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .Role)?
                                   .Value;

                var userId = int.Parse(User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .NameIdentifier)?
                                   .Value ?? "0");

                var existingBooking = await _context.booking.FindAsync(id);
                if (existingBooking == null)
                {
                    return NotFound("Booking not found");
                }

                if (string.Equals(userRole,"Customer") && existingBooking.UserId != userId)
                {
                    return Forbid();
                }

                if (string.Equals(userRole, "Customer"))
                {
                    booking.UserId = existingBooking.UserId;
                }

                var updatedBooking = await _booking.Update<Booking>(booking, id);

                _logger.LogInformation($"Booking {id} updated successfully");
                return Ok(updatedBooking);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, $"Error occurred while updating booking {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpPatch("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            try
            {
                var validStatuses = new[] { "Pending", "Confirmed",
                                            "CheckedIn", "CheckedOut",
                                            "Cancelled" };

                if (!validStatuses.Contains(status))
                {
                    _logger.LogWarning($"Invalid status '{status}' provided for booking {id}");
                    return BadRequest($"Invalid status. Valid values: " +
                                      $"{string.Join(", ", validStatuses)}");
                }

                var booking = await _context.booking.FindAsync(id);
                if (booking == null)
                {
                    _logger.LogWarning($"Booking {id} not found for status update");
                    return NotFound("Booking not found");
                }

                booking.Status = status;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking {id} status updated to '{status}' successfully");
                return Ok(booking);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while updating status for booking {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("DeleteBooking/{id}")]
        [Authorize(Roles = "Customer, Manager, Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var userRole = User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .Role)?
                                   .Value;

                var userId = int.Parse(User.FindFirst(System
                                   .Security
                                   .Claims
                                   .ClaimTypes
                                   .NameIdentifier)?
                                   .Value ?? "0");

                if (userRole is not "Manager" && userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized to remove a booking");
                }

                if (userId != id)
                {
                    return Unauthorized("Your not authorized add rooms");
                }

                var booking = await _context.booking.FindAsync(id);

                if (booking == null)
                {
                    return NotFound("Booking not found");
                }

                if (string.Equals(userRole, "Customer") && booking.UserId != userId)
                {
                    return Forbid();
                }

                if (string.Equals(userRole, "Customer") && !string.Equals(booking.Status, "Pending"))
                {
                    return BadRequest("Cannot cancel booking that is already confirmed or checked in");
                }

                var deletedBooking = await _booking.Delete<Booking>(id);

                var room = await _context.room.FindAsync(booking.RoomID);

                if (room != null)
                {
                    room.IsAvailable = true;
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation($"Booking {id} deleted successfully");
                return Ok(deletedBooking);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while deleting booking {id}");
                return BadRequest(x.Message);
            }
        }
    }
}
