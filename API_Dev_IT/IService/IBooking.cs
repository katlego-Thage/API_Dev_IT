using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IBooking
    {
        Task<T> Create<T>(Booking booking);
        Task<T> Update<T>(Booking booking, int id);
        Task<T> Delete<T>(int id);
        Task<bool> IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut);
        Task<decimal> CalculateTotalPrice(int roomId, DateTime checkIn, DateTime checkOut);
    }
}
