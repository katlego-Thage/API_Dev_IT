using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IRoom
    {
        Task<Room> Create(Room room);
        Task<Room> Update(Room room, int id);
        Task<Room> Delete(int id);
    }
}
