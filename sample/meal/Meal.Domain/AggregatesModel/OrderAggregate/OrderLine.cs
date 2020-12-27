using System;
using Repository.SeedWork;

namespace Meal.Domain.AggregatesModel.OrderAggregate
{
    public class OrderLine : BaseEntity1<Guid>
    {
        public Guid OrderId { get; private set; }

        public Order Order { get; private set; }

        public string Name { get; private set; }

        public decimal Price { get; private set; }

        public int Qty { get; private set; }

        public OrderLine()
        {
            Id = Guid.NewGuid();
        }

        public OrderLine(Guid orderId, string name, decimal price, int qty) : this()
        {
            OrderId = orderId;
            Name = name;
            Price = price;
            Qty = qty;
        }
    }
}
