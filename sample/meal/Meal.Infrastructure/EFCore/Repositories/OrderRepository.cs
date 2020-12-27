using Microsoft.EntityFrameworkCore;
using Repository.EFCore;
using Meal.Infrastructure.EFCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Meal.Domain.AggregatesModel.OrderAggregate;

namespace Meal.Infrastructure.EFCore.Repositories
{
    public class OrderRepository : EfRepository<Order, MealContext>, IOrderRepository
    {
        public OrderRepository(MealContext context) : base(context) { }

        public async Task<Order> GetOrderWithLines(Guid id)
        {
            return await _context.Orders
                .Include(x => x.OrderLines)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
