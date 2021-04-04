using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IInventoryRepository
    {
        Task Update(int productID, int quantity);
    }
}