using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IHandlerRepository
    {
        Task<int> Insert(HandlerModel data);
        Task Update(HandlerModel data);
    }
}