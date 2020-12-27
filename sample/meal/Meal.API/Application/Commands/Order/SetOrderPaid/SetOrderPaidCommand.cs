using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Application.Commands.PayOrder
{
    public class SetOrderPaidCommand : IRequest<bool>
    {
        public Guid OrderId { get; }

        public SetOrderPaidCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
