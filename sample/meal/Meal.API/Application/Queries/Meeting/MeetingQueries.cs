using AutoMapper;
using Dapper;
using Meal.Domain.AggregatesModel.MeetingAggregate;
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
    public class MeetingQueries : IMeetingQueries
    {
        private readonly MealContext _mealContext;
        private readonly IMapper _mapper;

        public MeetingQueries(MealContext mealContext, IMapper mapper)
        {
            _mealContext = mealContext ?? throw new ArgumentNullException(nameof(mealContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // A parameterless default constructor or one matching signature is required for materialization
        public async Task<List<MeetingDto>> GetMeetingsAsync()
        {
            //return _mapper.Map<List<MeetingDto>>(await _mealContext.Meetings
            //    .Include(x => x.Orders)
            //    .ThenInclude(x => x.OrderLines)
            //    .ToListAsync());

            var sql = @"select * from Meetings;
                        select * from Orders;
                        select * from OrderLines";

            using (var connection = await _mealContext.CreateDbConnectionAsync())
            {
                using (var multi = await connection.QueryMultipleAsync(sql))
                {
                    var meetings = (await multi.ReadAsync<Meeting>()).ToList();
                    var orders = (await multi.ReadAsync<Order>()).ToList();
                    var orderLines = (await multi.ReadAsync<OrderLine>()).ToList();

                    orders.ForEach(async o => o.AddLines(orderLines.Where(l => l.OrderId == o.Id)));

                    meetings.ForEach(m => m.AddOrders(orders.Where(o => o.MeetingId == m.Id)));

                    return _mapper.Map<List<MeetingDto>>(meetings);
                }
            }
        }
    }
}
