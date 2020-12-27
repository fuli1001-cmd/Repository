using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meal.Application.Commands.PayOrder;
using Meal.Domain.AggregatesModel.OrderAggregate;

namespace Meal.Application.Commands.SetOrderPaid
{
    public class SetOrderPaidCommandHandler : IRequestHandler<SetOrderPaidCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public SetOrderPaidCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> Handle(SetOrderPaidCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
                throw new ApplicationException($"Order {request.OrderId} does not exist.");

            order.SetPaid();

            _orderRepository.Update(order);

            return await _orderRepository.UnitOfWork.SaveAsync(cancellationToken);
        }
    }
}
