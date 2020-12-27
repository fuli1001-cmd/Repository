using Dapper.Contrib.Extensions;
using Repository.Attributes;
using Repository.SeedWork;
using Repository.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Dapper
{
    /// <summary>
    /// 以Dapper实现的Repository和UnitOfWork
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DapperRepository<TEntity> : IRepository<TEntity>
        where TEntity: BaseEntity, IAggregateRoot
    {
        public IUnitOfWork UnitOfWork { get; }

        public DapperRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Tracking]
        public async Task<TEntity> GetByIdAsync<T>(T id, CancellationToken cancellationToken = default)
        {
            using (var connection = await ((DapperUnitOfWork)UnitOfWork).CreateDbConnectionAsync())
            {
                return await connection.GetAsync<TEntity>(id);
            }
        }

        [Tracking]
        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = await ((DapperUnitOfWork)UnitOfWork).CreateDbConnectionAsync())
            {
                return (await connection.GetAllAsync<TEntity>()).ToList();
            }
        }

        [CheckEntity(true)]
        public void Add(TEntity entity)
        {
            ((UnitOfWork)UnitOfWork).Add(entity);
        }

        [CheckEntity]
        public void Update(TEntity entity)
        {
            ((UnitOfWork)UnitOfWork).Update(entity);
        }

        [CheckEntity]
        public void Remove(TEntity entity, bool cascade = false)
        {
            // cascade delete depends on database constraint.
            ((UnitOfWork)UnitOfWork).Remove(entity);
        }
    }
}
