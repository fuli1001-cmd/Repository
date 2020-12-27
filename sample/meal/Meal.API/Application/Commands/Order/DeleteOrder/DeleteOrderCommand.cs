using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; }

        public DeleteOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
