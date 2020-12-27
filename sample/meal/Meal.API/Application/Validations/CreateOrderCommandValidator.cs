using FluentValidation;
using Meal.Application.Commands.CreateOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meal.API.Application.Validations
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(cmd => cmd.EstimateAmount).LessThan(0).WithMessage("预估金额不能小于0");

            RuleForEach(cmd => cmd.OrderLines).SetValidator(new OrderItemDtoValidator());

            //RuleForEach(cmd => cmd.OrderLines).ChildRules(items =>
            //{
            //    items.RuleFor(item => item.Price).LessThan(0).WithMessage("菜品金额不能小于0");
            //    items.RuleFor(item => item.Qty).LessThanOrEqualTo(0).WithMessage("菜品数量必须大于0");
            //});
        }
    }

    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.Price).LessThan(0).WithMessage("菜品金额不能小于0");
            RuleFor(x => x.Qty).LessThanOrEqualTo(0).WithMessage("菜品数量必须大于0");
        }
    }
}
