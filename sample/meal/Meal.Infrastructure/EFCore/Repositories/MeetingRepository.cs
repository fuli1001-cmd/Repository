using Meal.Domain.AggregatesModel.MeetingAggregate;
using Repository.EFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Infrastructure.EFCore.Repositories
{
    public class MeetingRepository : EfRepository<Meeting, MealContext>, IMeetingRepository
    {
        public MeetingRepository(MealContext context) : base(context) { }
    }
}
