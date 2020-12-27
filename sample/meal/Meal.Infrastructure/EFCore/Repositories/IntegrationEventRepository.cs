using Repository.EFCore;
using Meal.Infrastructure.EFCore;
using System;
using System.Collections.Generic;
using System.Text;
using Meal.Domain.AggregatesModel.IntegrationEventAggregate;

namespace Meal.Infrastructure.EFCore.Repositories
{
    public class IntegrationEventRepository : EfRepository<IntegrationEvent, IntegrationContext>, IIntegrationEventRepository
    {
        public IntegrationEventRepository(IntegrationContext context) : base(context) { }
    }
}
