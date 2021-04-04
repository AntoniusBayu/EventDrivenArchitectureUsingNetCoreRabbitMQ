using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IOrderRepository
    {
        Task<int> Create(Order orderDetail);
        Task Delete(int orderId);
    }
}