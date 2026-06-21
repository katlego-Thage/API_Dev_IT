using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface ITenant
    {
        Task<T> Create<T>(Tenant ten);
        Task<T> Update<T>(Tenant ten, int id);
        Task<T> Delete<T>(int id);
    }
}
