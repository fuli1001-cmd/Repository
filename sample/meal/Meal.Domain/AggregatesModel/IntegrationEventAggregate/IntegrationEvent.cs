using Repository.SeedWork;
using System;

namespace Meal.Domain.AggregatesModel.IntegrationEventAggregate
{
    public class IntegrationEvent : BaseEntity1<Guid>, IAggregateRoot
    {
        public string Event { get; set; }

        public bool Published { get; set; }
    }
}
