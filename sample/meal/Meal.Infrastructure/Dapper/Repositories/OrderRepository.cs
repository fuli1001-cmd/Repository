using Dapper;
using Repository.Attributes;
using Repository.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meal.Domain.AggregatesModel.OrderAggregate;

namespace Meal.Infrastructure.Dapper.Repositories
{
    public class OrderRepository : DapperRepository<Order>, IOrderRepository
    {
        public OrderRepository(MealContext context) : base(context) { }

        [Tracking]
        public async Task<Order> GetOrderWithLines(Guid id)
        {
            //// way 1, use QueryAsync
            //var lookup = new Dictionary<Guid, Order>();
            //var sql = @"select * from orders o inner join orderlines ol
            //            on o.Id = ol.OrderId where o.Id = @OrderId";

            //using (var connection = await ((DapperUnitOfWork)UnitOfWork).CreateDbConnectionAsync())
            //{
            //    var orders = await connection.QueryAsync<Order, OrderLine, Order>(sql,
            //        (o, line) =>
            //        {
            //            Order order;
            //            if (!lookup.TryGetValue(o.Id, out order))
            //                lookup.Add(o.Id, order = o);

            //            order.AddLine(line);

            //            return o;
            //        },
            //        new { OrderId = id });

            //    return lookup.Values.First();
            //}

            //way 2, use QueryMultipleAsync
            var sql = @"select * from orders where Id = @OrderId;
                        select * from orderlines where OrderId = @OrderId";

            using (var connection = await ((DapperUnitOfWork)UnitOfWork).CreateDbConnectionAsync())
            {
                using (var multi = await connection.QueryMultipleAsync(sql, new { Id = id, OrderId = id }))
                {
                    var order = (await multi.ReadAsync<Order>()).First();

                    (await multi.ReadAsync<OrderLine>())
                        .ToList()
                        .ForEach(line => order.AddLine(line));

                    return order;
                }
            }
        }
    }
}
