using Meal.Domain.AggregatesModel.MeetingAggregate;
using Repository.Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Infrastructure.Dapper.Repositories
{
    public class MeetingRepository : DapperRepository<Meeting>, IMeetingRepository
    {
        public MeetingRepository(MealContext context) : base(context) { }
    }
}
