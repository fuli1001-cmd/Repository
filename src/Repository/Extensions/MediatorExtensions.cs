using MediatR;
using Repository.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, UnitOfWork uow)
        {
            var domainEntities = uow.EntityTrackers
                .Where(et => (et.Entity.DomainEvents?.Count ?? 0) > 0);

            var domainEvents = domainEntities
                .SelectMany(et => et.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(et => et.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
