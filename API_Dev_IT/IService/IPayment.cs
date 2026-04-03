using API_Dev_IT.Model;

namespace API_Dev_IT.IService
{
    public interface IPayment
    {
        Task<Payment> Create(Payment pay);
        Task<Payment> Update(Payment pay, int id);
        Task<Payment> Delete(int id);
    }
}
