using Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Domain.AggregatesModel.IntegrationEventAggregate
{
    public interface IIntegrationEventRepository : IRepository<IntegrationEvent>
    {
    }
}
