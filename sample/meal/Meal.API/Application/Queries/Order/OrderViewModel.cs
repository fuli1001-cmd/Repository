using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meal.API.Application.Queries
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderLineDto> OrderLines { get; set; }
    }

    public class OrderLineDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        Paid,
        Shipping,
        Finished
    }
}
