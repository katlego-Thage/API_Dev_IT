using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Service
{
    public class BookingService : IBooking
    {
        private readonly BookingContext _context;
        private readonly ILogger<TenantService> _logger;
        public BookingService(BookingContext context,
               ILogger<TenantService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Booking> Create(Booking booking)
        {
            var tenantExists = await _context.tenant
                              .AnyAsync(t => 
                              t.TenantID == booking.TenantID);

            if (!tenantExists)
            {
                throw new InvalidOperationException("Tenant not found");
            }

            var room = await _context.room.FindAsync(booking.BookingID);

            if (room is null)
            {
                throw new InvalidOperationException("Room not found");
            }

            if (booking.CheckInDate >= booking.CheckOutDate)
            {
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
                throw new InvalidOperationException(
                          "Room is not available for selected dates");
            }

            _context.booking.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }
        public async Task<Booking> Update(Booking booking, int id)
        {
            var existingBooking = await _context.booking.FindAsync(id);

            if (existingBooking is null)
            {
                throw new InvalidOperationException("Booking not found");
            }

            existingBooking.TenantID = booking.TenantID;
            existingBooking.RoomID = booking.RoomID;
            existingBooking.CheckInDate = booking.CheckInDate;
            existingBooking.CheckOutDate = booking.CheckOutDate;
            existingBooking.Status = booking.Status?? existingBooking.Status;
            existingBooking.UserId = booking.UserId;

            await _context.SaveChangesAsync();

            return existingBooking;
        }

        public async Task<Booking> Delete(int id)
        {
            var booking = await _context.booking
                         .FirstOrDefaultAsync(x => 
                         x.RoomID == id);

            if (booking is null)
            {
                throw new InvalidOperationException("Booking not found");
            }

            _context.booking.Remove(booking);
            await _context.SaveChangesAsync();

            return booking;
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
