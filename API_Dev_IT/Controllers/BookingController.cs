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
        private readonly ILogger<PaymentController> _logger;
        private readonly IBooking _booking;
        private readonly UserRoleHelper _helper;
        public BookingController(BookingContext context,
               IPayment payment,
               ILogger<PaymentController> logger,
               IBooking booking,
               UserRoleHelper helper)
        {
            _context = context;
            _payment = payment;
            _logger = logger;
            _booking = booking;
            _helper = helper;
        }

        [HttpGet("GetBooking")]
        [Authorize(Roles = "Manager, Receptionist, Admin, Customer")]
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

                return Ok(allBookings);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetBooking/{id}")]
        [Authorize(Roles = "Manager, Receptionist, Admin, Customer")]
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

                if (userRole == "Customer" && booking.UserId != userId)
                {
                    return Forbid();
                    //return BadRequest(x.Message);
                }

                return Ok(booking);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
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

                // What i need to do: Add date range conflict checking with existing bookings
                // Note: This requires checking Booking table for overlapping dates

                var availableRooms = await query.ToListAsync();
                return Ok(availableRooms);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetBookingDetails/{id}")]
        //[Authorize(Roles = "Manager, Receptionist, Admin, Customer")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            try
            {
                //var userRole = User.FindFirst(System.Security.Claims
                //               .ClaimTypes.Role)?.Value;

                //var userId = int.Parse(User.FindFirst(System.Security
                //                   .Claims.ClaimTypes.NameIdentifier)?
                //                   .Value ?? "0");

                //if (userRole != "Manager" || userRole != "Admin" ||
                //    userRole != "Customer" || userRole != "Receptionist")
                //{
                //    return Unauthorized("Your not authorized to view a booking");
                //}

                //if (userId != id)
                //{
                //    return Unauthorized("Your not authorized add rooms");
                //}

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

                return Ok(bookingDetails);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("CreateBooking")]
        [Authorize(Roles = "Customer, Receptionist, Manager, Admin")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            try
            {
                //var userRole = User.FindFirst(System
                //                   .Security
                //                   .Claims
                //                   .ClaimTypes
                //                   .Role)?
                //                   .Value;

                //var userId = int.Parse(User.FindFirst(System
                //                   .Security
                //                   .Claims
                //                   .ClaimTypes
                //                   .NameIdentifier)?
                //                   .Value ?? "0");
                //var userRole = _booking.GetRole();
                //if (userRole is not "Manager" || userRole is not "Admin")
                //{
                //    return Unauthorized("Your not authorized to remove a booking");
                //}

                //var userId = UserRoleHelper.GetUserId();
                //if (userId != booking.UserId)
                //{
                //    return Unauthorized("Your not authorized add rooms");
                //}

                //if (userRole == "Customer")
                //{
                //    booking.UserId = userId;
                //}

                var room = await _context.room.FindAsync(booking.RoomID);

                if (room == null || room.IsAvailable == false)
                {
                    return BadRequest("Room is not available");
                }

                booking.BookingDate = DateTime.Now;
                booking.Status = "Pending";

                var createdBooking = await _booking.Create(booking);

                return Ok(createdBooking);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("UpdateBooking/{id}")]
        [Authorize(Roles = "Customer, Receptionist, Manager, Admin")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking booking)
        {
            try
            {
                //var userRole = User.FindFirst(System
                //                   .Security
                //                   .Claims
                //                   .ClaimTypes
                //                   .Role)?
                //                   .Value;

                //var userId = int.Parse(User.FindFirst(System
                //                   .Security
                //                   .Claims
                //                   .ClaimTypes
                //                   .NameIdentifier)?
                //                   .Value ?? "0");
                //var userRole = UserRoleHelper.GetRole();
                //if (userRole is not "Manager" || userRole is not "Admin")
                //{
                //    return Unauthorized("Your not authorized to remove a booking");
                //}

                //var userId = UserRoleHelper.GetUserId();
                //if (userId != id)
                //{
                //    return Unauthorized("Your not authorized add rooms");
                //}

                var existingBooking = await _context.booking.FindAsync(id);
                if (existingBooking == null)
                {
                    return NotFound("Booking not found");
                }

                //if (userRole == "Customer" && existingBooking.UserId != userId)
                //{
                //    return Forbid();
                //}

                //if (userRole == "Customer")
                //{
                //    booking.UserId = existingBooking.UserId;
                //}

                var updatedBooking = await _booking.Update(booking, id);

                return Ok(updatedBooking);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPatch("UpdateStatus/{id}")]
        [Authorize(Roles = "Receptionist, Manager, Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            try
            {
                var validStatuses = new[] { "Pending", "Confirmed",
                                            "CheckedIn", "CheckedOut",
                                            "Cancelled" };

                if (!validStatuses.Contains(status))
                {
                    return BadRequest($"Invalid status. Valid values: " +
                                      $"{string.Join(", ", validStatuses)}");
                }

                var booking = await _context.booking.FindAsync(id);
                if (booking == null)
                {
                    return NotFound("Booking not found");
                }

                booking.Status = status;
                await _context.SaveChangesAsync();

                return Ok(booking);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("DeleteBooking/{id}")]
        [Authorize(Roles = "Customer, Manager, Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                //var userRole = User.FindFirst(System
                //                   .Security
                //                   .Claims
                //                   .ClaimTypes
                //                   .Role)?
                //                   .Value;

                //var userId = int.Parse(User.FindFirst(System
                //                   .Security
                //                   .Claims
                //                   .ClaimTypes
                //                   .NameIdentifier)?
                //                   .Value ?? "0");
                //var userRole = UserRoleHelper.GetRole();
                //if (userRole is not "Manager" || userRole is not "Admin")
                //{
                //    return Unauthorized("Your not authorized to remove a booking");
                //}

                //var userId = UserRoleHelper.GetUserId();
                //if (userId != id)
                //{
                //    return Unauthorized("Your not authorized add rooms");
                //}

                var booking = await _context.booking.FindAsync(id);

                if (booking == null)
                {
                    return NotFound("Booking not found");
                }

                //if (userRole == "Customer" && booking.UserId != userId)
                //{
                //    return Forbid();
                //}

                //if (userRole == "Customer" && booking.Status != "Pending")
                //{
                //    return BadRequest("Cannot cancel booking that is already confirmed or checked in");
                //}

                var deletedBooking = await _booking.Delete(id);

                var room = await _context.room.FindAsync(booking.RoomID);

                if (room != null)
                {
                    room.IsAvailable = true;
                    await _context.SaveChangesAsync();
                }

                return Ok(deletedBooking);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }
    }
}
