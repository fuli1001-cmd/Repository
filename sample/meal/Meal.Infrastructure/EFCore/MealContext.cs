using MediatR;
using Microsoft.EntityFrameworkCore;
using Repository.EFCore;
using System;
using Meal.Domain.AggregatesModel.OrderAggregate;
using Meal.Domain.AggregatesModel.MeetingAggregate;

namespace Meal.Infrastructure.EFCore
{
    public class MealContext : EFUnitOfWork
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderLine> OrderLines { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        public MealContext(DbContextOptions<MealContext> options, IMediator mediator) 
            : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Ignore(e => e.DomainEvents);
            modelBuilder.Entity<OrderLine>().Ignore(e => e.DomainEvents);
            modelBuilder.Entity<Meeting>().Ignore(e => e.DomainEvents);

            modelBuilder.Entity<Meeting>().Metadata
                .FindNavigation(nameof(Meeting.Orders))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Order>().Metadata
                .FindNavigation(nameof(Order.OrderLines))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            //modelBuilder.Entity<Order>().HasMany(c => c.OrderLines).WithOne(uc => uc.Order).HasForeignKey(uc => uc.OrderId).OnDelete(DeleteBehavior.Restrict);

            //SeedData(modelBuilder);
        }

        //private void SeedData(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Meeting>().HasData(
        //        new Meeting { Id = Guid.Parse("46979f55-b071-7423-528d-6d04396731d3"), Name = "Microservices Patterns: With examples in Java", Price = 54.50M },
        //        new Meeting { Id = Guid.Parse("9511a41a-b82b-0ad5-f504-d242d5fc15af"), Name = "Building Event-Driven Microservices: Leveraging Organizational Data at Scale", Price = 48.12M });
        //}
    }
}
