using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Meal.Domain.AggregatesModel.IntegrationEventAggregate;
using Meal.Domain.AggregatesModel.OrderAggregate;
using Meal.Domain.AggregatesModel.MeetingAggregate;
using AutoMapper;
using Meal.API.Application.Queries;

namespace Meal.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        //private readonly IIntegrationEventRepository _integrationRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository,
            IMeetingRepository meetingRepository, 
            IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _meetingRepository = meetingRepository ?? throw new ArgumentNullException(nameof(meetingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            //_integrationRepository = integrationRepository ?? throw new ArgumentNullException(nameof(integrationRepository));
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetByIdAsync(request.MeetingId);

            var order = meeting.CreateOrder(request.EstimateAmount);

            foreach (var line in request.OrderLines)
            {
                order.AddItem(line.Name, line.Price, line.Qty);
            }

            _orderRepository.Add(order);

            await _orderRepository.UnitOfWork.SaveAsync(cancellationToken);

            //// distributed transaction
            //var @event = new IntegrationEvent { Event = "OrderCreatedEvent", Published = false };
            //_integrationRepository.Add(@event);

            ////await ResilientTransaction.New(((DbContext)_orderRepository.UnitOfWork))
            ////    .ExecuteAsync(async () =>
            ////    {
            ////        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
            ////            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            ////            TransactionScopeAsyncFlowOption.Enabled))
            ////        {
            ////            await _orderRepository.UnitOfWork.SaveAsync(cancellationToken);
            ////            throw new ApplicationException("----- rollback!!!");
            ////            //await _integrationRepository.UnitOfWork.SaveAsync(cancellationToken);

            ////            scope.Complete();
            ////        }
            ////    });

            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
            //            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            //            TransactionScopeAsyncFlowOption.Enabled))
            //{
            //    await _orderRepository.UnitOfWork.SaveAsync(cancellationToken);
            //    //throw new ApplicationException("----- rollback!!!");
            //    await _integrationRepository.UnitOfWork.SaveAsync(cancellationToken);

            //    scope.Complete();
            //}

            return _mapper.Map<OrderDto>(order);
        }
    }
}
