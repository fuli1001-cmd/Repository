using System.Threading;
using System.Threading.Tasks;

namespace Repository.UOW
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync(CancellationToken cancellationToken = default);
    }
}
