using MediatR;
using Microsoft.EntityFrameworkCore;
using Repository.EFCore;
using System;
using System.Collections.Generic;
using System.Text;
using Meal.Domain.AggregatesModel.IntegrationEventAggregate;

namespace Meal.Infrastructure.EFCore
{
    public class IntegrationContext : EFUnitOfWork
    {
        public DbSet<IntegrationEvent> IntegrationEvents { get; set; }

        public IntegrationContext(DbContextOptions<IntegrationContext> options, IMediator mediator)
            : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationEvent>().Ignore(e => e.DomainEvents);
        }
    }
}
