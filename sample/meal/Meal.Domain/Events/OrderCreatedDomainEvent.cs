using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Domain.Events
{
    public class OrderCreatedDomainEvent : INotification
    {
        public Guid MeetingId { get; }

        public decimal EstimateAmount { get; }

        public OrderCreatedDomainEvent(Guid meetingId, decimal estimateAmount)
        {
            MeetingId = meetingId;
            EstimateAmount = estimateAmount;
        }
    }
}
