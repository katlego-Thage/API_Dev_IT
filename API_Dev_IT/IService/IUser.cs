using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IUser
    {
        Task<User> LogIn(LogIn logIn);
    }
}
