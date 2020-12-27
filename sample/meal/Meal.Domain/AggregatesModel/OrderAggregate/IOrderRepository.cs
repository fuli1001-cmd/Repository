using Repository;
using System;
using System.Threading.Tasks;

namespace Meal.Domain.AggregatesModel.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderWithLines(Guid id);
    }
}
