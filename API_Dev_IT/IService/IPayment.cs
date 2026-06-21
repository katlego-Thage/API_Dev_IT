using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IPayment
    {
        Task<T> Create<T>(Payment pay);
        Task<T> Update<T>(Payment pay, int id);
    }
}
