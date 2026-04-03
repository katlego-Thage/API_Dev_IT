using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IBooking
    {
        Task<Booking> Create(Booking booking);
        Task<Booking> Update(Booking booking, int id);
        Task<Booking> Delete(int id);
        Task<bool> IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut);
        Task<decimal> CalculateTotalPrice(int roomId, DateTime checkIn, DateTime checkOut);
    }
}
