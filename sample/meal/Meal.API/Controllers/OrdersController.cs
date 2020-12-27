using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meal.Application.Commands.CreateOrder;
using Meal.API.Application.Queries;
using System.Collections.Generic;
using Meal.Application.Commands.DeleteOrder;

namespace Meal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrderAsync(CreateOrderCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrderAsync()
        {
            return await _orderQueries.GetOrdersAsync();
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult<bool>> DeleteOrderAsync(Guid orderId)
        {
            return await _mediator.Send(new DeleteOrderCommand(orderId));
        }
    }
}
