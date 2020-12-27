using Meal.Domain.AggregatesModel.MeetingAggregate;
using Meal.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meal.API.Application.DomainEventHandlers.OrderStarted
{
    public class ChangeMeetingStatusWhenOrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ILogger<ChangeMeetingStatusWhenOrderCreatedDomainEventHandler> _logger;

        public ChangeMeetingStatusWhenOrderCreatedDomainEventHandler(IMeetingRepository meetingRepository, ILogger<ChangeMeetingStatusWhenOrderCreatedDomainEventHandler> logger)
        {
            _meetingRepository = meetingRepository ?? throw new ArgumentNullException(nameof(meetingRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetByIdAsync(notification.MeetingId);
            meeting.IncreaseEstimateAmount(notification.EstimateAmount);
            await _meetingRepository.UnitOfWork.SaveAsync(cancellationToken);
        }
    }
}
