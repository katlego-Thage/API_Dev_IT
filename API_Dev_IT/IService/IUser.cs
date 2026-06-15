using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IUser
    {
        Task<T> LogIn<T> (LogIn logIn);
        Task<T> Create<T>(User user);
        Task<User> Update(User user, int id);
        Task<User> Delete(int id);
    }
}
