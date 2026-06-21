using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Service
{
    public class PaymentService : IPayment
    {
        private readonly BookingContext _context;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(BookingContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> Create<T>(Payment room)
        {
            try
            {
                var rooms = await _context.payment
                           .FirstOrDefaultAsync(x => 
                           x.PaymentID == room.PaymentID);
                
                if (rooms?.PaymentID == room.PaymentID)
                {
                    _logger.LogWarning($"Failed: payment for {room.BookingID}" +
                                       $" already exists");
                    throw new InvalidOperationException(
                              "Payment already made");
                }

                var insert = new Payment
                {
                    BookingID = room.BookingID,
                    Amount = room.Amount,
                    PaymentDate = room.PaymentDate,
                    PaymentMethod = room.PaymentMethod,
                };

                _context.payment.Add(insert);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                $"Tenant {insert.BookingID} created successfully");
                return (T)(object)insert;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during tenant registration");
                throw;
            }
        }

        public async Task<T> Update<T>(Payment room, int id)
        {
            try
            {
                var rooms = await _context.payment
                            .FirstOrDefaultAsync(x => 
                            x.BookingID == id);

                if (rooms?.BookingID != room.BookingID)
                {
                    _logger.LogWarning($" email {room.BookingID}" +
                                       $" doesnt exists");
                    throw new InvalidOperationException(
                              "Invalid Room");
                }

                rooms.BookingID = room.BookingID;
                rooms.Amount = room.Amount;
                rooms.PaymentDate = room.PaymentDate;
                rooms.PaymentMethod = room.PaymentMethod;

                _context.Update(rooms);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tenant {id} updated successfully");
                return (T)(object)rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Tenant error occurred while updating {id}");
                throw;
            }
        }
    }
}
