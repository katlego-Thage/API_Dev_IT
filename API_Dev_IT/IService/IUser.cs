using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IUser
    {
        Task<T> LogIn<T>(LogIn login);
        Task<T> Create<T>(User register);
        Task<T> Update<T>(User user, int id);
        Task<T> Delete<T>(int id);
    }
}
