using MediatR;
using System;
using System.Collections.Generic;
using Meal.API.Application.Queries;

namespace Meal.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public Guid MeetingId { get; }

        public decimal EstimateAmount { get; }

        public List<OrderItemDto> OrderLines { get; }

        public CreateOrderCommand(Guid meetingId, decimal estimateAmount, List<OrderItemDto> orderLines)
        {
            MeetingId = meetingId;
            EstimateAmount = estimateAmount;
            OrderLines = orderLines;
        }
    }

    public class OrderItemDto
    {
        public string Name { get; }

        public decimal Price { get; }

        public int Qty { get; }

        public OrderItemDto(string name, decimal price, int qty)
        {
            Name = name;
            Price = price;
            Qty = qty;
        }
    }
}
