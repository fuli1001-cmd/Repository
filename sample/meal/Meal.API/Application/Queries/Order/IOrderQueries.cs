﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meal.API.Application.Queries
{
    public interface IOrderQueries
    {
        Task<List<OrderDto>> GetOrdersAsync();
    }
}
