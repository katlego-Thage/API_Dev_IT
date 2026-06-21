using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Service
{
    public class BookingService : IBooking
    {
        private readonly BookingContext _context;
        private readonly ILogger<BookingService> _logger;
        public BookingService(BookingContext context,
               ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<T> Create<T>(Booking booking)
        {
            try
            {
                var tenantExists = await _context.tenant
                  .AnyAsync(t =>
                  t.TenantID == booking.TenantID);

                if (!tenantExists)
                {
                    _logger.LogWarning($"Failed: room {booking.TenantID} not found");
                    throw new InvalidOperationException("Tenant not found");
                }

                var room = await _context.room.FindAsync(booking.BookingID);

                if (room is null)
                {
                    _logger.LogWarning($"Failed: room {booking.RoomID} not found");
                    throw new InvalidOperationException("Room not found");
                }

                if (booking.CheckInDate >= booking.CheckOutDate)
                {
                    _logger.LogWarning($"Failed: invalid dates for booking {booking.BookingID}");
                    throw new InvalidOperationException(
                              "Check-out date must be after check-in date");
                }

                var hasConflict = await _context.booking
                    .AnyAsync(b => b.RoomID == booking.RoomID
                        && b.Status != "Cancelled"
                        && b.CheckInDate < booking.CheckOutDate
                        && b.CheckOutDate > booking.CheckInDate);

                if (hasConflict)
                {
                    _logger.LogWarning($"Failed: room {booking.RoomID} is not available for selected dates");
                    throw new InvalidOperationException(
                              "Room is not available for selected dates");
                }

                _context.booking.Add(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking {booking.BookingID} created successfully");
                return (T)(object)booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                throw;
            }
        }

        public async Task<T> Update<T>(Booking booking, int id)
        {
            try
            {
                var existingBooking = await _context.booking.FindAsync(id);

                if (existingBooking is null)
                {
                    _logger.LogWarning($"Failed: booking {id} not found for update");
                    throw new InvalidOperationException("Booking not found");
                }

                existingBooking.TenantID = booking.TenantID;
                existingBooking.RoomID = booking.RoomID;
                existingBooking.CheckInDate = booking.CheckInDate;
                existingBooking.CheckOutDate = booking.CheckOutDate;
                existingBooking.Status = booking.Status ?? existingBooking.Status;
                existingBooking.UserId = booking.UserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking {id} updated successfully");
                return (T)(object)existingBooking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating booking {id}");
                throw;
            }
        }

        public async Task<T> Delete<T>(int id)
        {
            try
            {
                var booking = await _context.booking
                             .FirstOrDefaultAsync(x =>
                             x.RoomID == id);

                if (booking is null)
                {
                    _logger.LogWarning($"Failed: booking {id} not found for deletion");
                    throw new InvalidOperationException("Booking not found");
                }

                _context.booking.Remove(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking {id} deleted successfully");

                return (T)(object)booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting booking {id}");
                throw;
            }
        }

        public async Task<bool> IsRoomAvailable(int roomId, 
                                DateTime checkIn, DateTime checkOut)
        {
            return !await _context.booking
                .AnyAsync(b => b.RoomID == roomId
                    && b.Status != "Cancelled"
                    && b.CheckInDate < checkOut
                    && b.CheckOutDate > checkIn);
        }

        public async Task<decimal> CalculateTotalPrice(int roomId, 
                                   DateTime checkIn, DateTime checkOut)
        {
            var room = await _context.room.FindAsync(roomId);

            if (room is null || room.PricePerNight is null)
            {
                return 0;
            }

            var nights = (checkOut - checkIn).Days;

            return room.PricePerNight.Value * nights;
        }
    }
}
