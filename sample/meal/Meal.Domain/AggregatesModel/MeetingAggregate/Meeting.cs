using Meal.Domain.AggregatesModel.OrderAggregate;
using Meal.Domain.Exceptions;
using Repository.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Domain.AggregatesModel.MeetingAggregate
{
    public class Meeting : BaseEntity1<Guid>, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Address { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public decimal EstimateAmount { get; private set; }

        public decimal ActualAmount { get; private set; }

        private readonly List<Order> _orders = null;

        public IReadOnlyCollection<Order> Orders => _orders;

        internal Meeting() 
        {
            _orders = new List<Order>();
        }

        public Meeting(string name, string address, DateTime startDate, DateTime endDate) : this()
        {
            Name = name;
            Address = address;
            StartDate = startDate;
            EndDate = endDate;
        }

        public Order CreateOrder(decimal estimateAmount)
        {
            // invariant, business rule checking
            if (DateTime.Today < StartDate)
                throw new MealDomainException("会议尚未开始。");

            if (DateTime.Today > EndDate)
                throw new MealDomainException("会议已经结束。");

            var order = new Order(Id, estimateAmount);

            EstimateAmount = estimateAmount;

            return order;
        }

        public void IncreaseEstimateAmount(decimal amount)
        {
            EstimateAmount += amount;
        }

        public void AddOrders(IEnumerable<Order> orders)
        {
            _orders.AddRange(orders);
        }

        public void AddOrder(Order order)
        {
            _orders.Add(order);
        }
    }
}
