using Meal.Domain.AggregatesModel.OrderAggregate;
using AutoMapper;

namespace Meal.API.Application.Queries
{
    public class OrderViewModelProfile : Profile
    {
        public OrderViewModelProfile()
        {
            CreateMap<OrderLine, OrderLineDto>();
            CreateMap<Order, OrderDto>();
        }
    }
}
