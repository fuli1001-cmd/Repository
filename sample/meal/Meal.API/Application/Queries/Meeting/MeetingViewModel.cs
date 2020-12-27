using System;
using System.Collections.Generic;

namespace Meal.API.Application.Queries
{
    public class MeetingDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal EstimateAmount { get; set; }

        public decimal ActualAmount { get; set; }

        public List<OrderDto> Orders { get; set; }
    }
}
