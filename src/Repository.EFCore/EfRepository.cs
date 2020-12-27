using Microsoft.EntityFrameworkCore;
using Repository.SeedWork;
using Repository.UOW;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.EFCore
{
    public class EfRepository<T, TC> : IRepository<T>
        where T : BaseEntity, IAggregateRoot
        where TC : DbContext, IUnitOfWork
    {
        protected readonly TC _context;
        public IUnitOfWork UnitOfWork => _context;

        public EfRepository(TC context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync<T1>(T1 id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity, bool cascade = false)
        {
            _context.Set<T>().Remove(entity);
        }

        //public async Task<List<T>> GetAsync(ISpecification<T> spec)
        //{
        //    return await ApplySpecification(spec).ToListAsync();
        //}

        //private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        //{
        //    return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        //}
    }
}
