using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Dev_IT.Helper
{
    public class UserRoleHelper //note to self: not sure will use this, but leave it as the code expands 
    {
        private readonly BookingContext _context;
        public UserRoleHelper(BookingContext context)
        {
            _context = context;
        }
        public async Task<object> BookingDetails(int id)
        {
            var bookingDetails = await _context.booking
                .Where(b => b.BookingID == id)
                .Select(b => new
                {
                    b.BookingID,
                    b.BookingDate,
                    b.CheckInDate,
                    b.CheckOutDate,
                    b.Status,
                    Tenant = _context.tenant
                        .Where(t => t.TenantID == b.TenantID)
                        .Select(t => new { t.FullName, t.Phone, t.Email })
                        .FirstOrDefault(),
                    Room = _context.room
                        .Where(r => r.RoomID == b.RoomID)
                        .Select(r => new { r.RoomNumber, r.RoomType, r.PricePerNight })
                        .FirstOrDefault(),
                    Payment = _context.payment
                        .Where(p => p.BookingID == b.BookingID)
                        .Select(p => new { p.Amount, p.PaymentDate, p.PaymentMethod })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
            return bookingDetails;
        }
    }
}