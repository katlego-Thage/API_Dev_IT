using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IUser
    {
        //Task<User> LogIn(LogIn logIn);

        Task<T> LogIn<T> (LogIn logIn);
            //public List<T> GetItems<T>()
        Task<User> Create(User user);
        Task<User> Update(User user, int id);
        Task<User> Delete(int id);
    }
}
