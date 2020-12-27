using AutoMapper;
using Dapper;
using Meal.Domain.AggregatesModel.OrderAggregate;
using Meal.Infrastructure.Dapper;
//using Meal.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meal.API.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private readonly MealContext _mealContext;
        private readonly IMapper _mapper;

        public OrderQueries(MealContext mealContext, IMapper mapper)
        {
            _mealContext = mealContext ?? throw new ArgumentNullException(nameof(mealContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<OrderDto>> GetOrdersAsync()
        {
            //return _mapper.Map<List<OrderDto>>(await _mealContext.Orders
            //    .Include(x => x.OrderLines)
            //    .ToListAsync());

            var sql = @"select * from Orders;
                        select * from OrderLines";

            using (var connection = await _mealContext.CreateDbConnectionAsync())
            {
                using (var multi = await connection.QueryMultipleAsync(sql))
                {
                    var orders = (await multi.ReadAsync<Order>()).ToList();
                    var orderLines = (await multi.ReadAsync<OrderLine>()).ToList();

                    orders.ForEach(o => o.AddLines(orderLines.Where(l => l.OrderId == o.Id)));

                    return _mapper.Map<List<OrderDto>>(orders);
                }
            }
        }
    }
}
