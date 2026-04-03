using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Service
{
    public class PaymentService : IPayment
    {
        private readonly BookingContext _context;
        private readonly ILogger<TenantService> _logger;
        public PaymentService(BookingContext context,
               ILogger<TenantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Payment> Create(Payment room)
        {
            try
            {
                var rooms = await _context.payment
                           .FirstOrDefaultAsync(x => 
                           x.PaymentID == room.PaymentID);
                
                if (rooms?.PaymentID == room.PaymentID)
                {
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

                return insert;

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }

        public async Task<Payment> Update(Payment room, int id)
        {
            try
            {
                var rooms = await _context.payment
                            .FirstOrDefaultAsync(x => 
                            x.BookingID == id);

                if (rooms?.BookingID != room.BookingID)
                {
                    throw new InvalidOperationException(
                              "Invalid Room");
                }

                rooms.BookingID = room.BookingID;
                rooms.Amount = room.Amount;
                rooms.PaymentDate = room.PaymentDate;
                rooms.PaymentMethod = room.PaymentMethod;

                _context.Update(rooms);
                await _context.SaveChangesAsync();

                return rooms;
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }

        public async Task<Payment> Delete(int id)
        {
            try 
            {
                var room = await _context.payment
                          .FirstOrDefaultAsync(x =>
                          x.PaymentID == id);

                if (room is null || room.PaymentID != id)
                {
                    throw new InvalidOperationException(
                              "Room doesnt exist");
                }

                _context.payment.Remove(room);
                await _context.SaveChangesAsync();

                return room;
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }
    }
}
