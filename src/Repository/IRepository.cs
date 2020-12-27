using Repository.SeedWork;
using Repository.UOW;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    /// <summary>
    /// Repository模式接口，与实现方式无关
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<TEntity> GetByIdAsync<T>(T id, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity, bool cascade = false);
    }
}
