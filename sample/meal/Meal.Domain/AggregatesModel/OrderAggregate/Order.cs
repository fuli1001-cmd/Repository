using Meal.Domain.AggregatesModel.MeetingAggregate;
using Meal.Domain.Events;
using Meal.Domain.Exceptions;
using Repository.SeedWork;
using System;
using System.Collections.Generic;

namespace Meal.Domain.AggregatesModel.OrderAggregate
{
    public class Order : BaseEntity1<Guid>, IAggregateRoot
    {
        public decimal EstimateAmount { get; private set; }

        public decimal ActualAmount { get; private set; }

        public DateTime CreateTime { get; private set; }

        public OrderStatus Status { get; private set; }

        private readonly List<OrderLine> _orderLines = null;

        public IReadOnlyCollection<OrderLine> OrderLines => _orderLines;

        public Guid MeetingId { get; private set; }

        internal Order()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.UtcNow;
            Status = OrderStatus.Created;
            _orderLines = new List<OrderLine>();
        }

        internal Order(Guid meetingId, decimal estimateAmount) : this()
        {
            MeetingId = meetingId;
            EstimateAmount = estimateAmount;
        }

        public void AddLine(OrderLine line)
        {
            _orderLines.Add(line);
        }

        public void AddLines(IEnumerable<OrderLine> lines)
        {
            _orderLines.AddRange(lines);
        }

        // 添加订单项
        public void AddItem(string name, decimal price, int qty)
        {
            // invariant, business rule checking
            if (Status != OrderStatus.Created && Status != OrderStatus.Ordered)
                throw new MealDomainException($"订单已处于{Status}状态，不能下单。");

            var totalAmount = ActualAmount + price * qty;

            if (totalAmount > EstimateAmount)
                throw new MealDomainException($"总金额不能超过{EstimateAmount}元。");

            // 添加订单项
            var orderLine = new OrderLine(Id, name, price, qty);
            _orderLines.Add(orderLine);

            // 更新订单价格，设置订单状态为已下单
            ActualAmount += price * qty;
            Status = OrderStatus.Ordered;

            // 添加已下单的domain event
            AddOrderedDomainEvent();
        }

        public void Remove()
        {
            if (Status != OrderStatus.Created)
                throw new MealDomainException($"订单已经开始，不能删除");

            AddOrderDeletedDomainEvent();
        }

        // 设置订单为已付款
        public void SetPaid()
        {
            if (Status != OrderStatus.Created)
                throw new MealDomainException($"Order is in {Status} status.");

            Status = OrderStatus.Paid;
        }

        private void AddOrderedDomainEvent()
        {
            var @event = new OrderCreatedDomainEvent(MeetingId, EstimateAmount);
            AddDomainEvent(@event);
        }

        private void AddOrderDeletedDomainEvent()
        {
            var @event = new OrderDeletedDomainEvent(MeetingId, EstimateAmount);
            AddDomainEvent(@event);
        }
    }

    public enum OrderStatus
    {
        Created,
        Ordered,
        Paid,
        Canceled
    }
}
