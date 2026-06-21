using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IRoom
    {
        Task<T> Create<T>(Room room);
        Task<T> Update<T>(Room room, int id);
        Task<T> Delete<T>(int id);
    }
}
