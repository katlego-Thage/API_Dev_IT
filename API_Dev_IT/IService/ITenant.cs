using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface ITenant
    {
        Task<Tenant> Create(Tenant ten);
        Task<Tenant> Update(Tenant ten, int id);
        Task<Tenant> Delete(int id);
    }
}
