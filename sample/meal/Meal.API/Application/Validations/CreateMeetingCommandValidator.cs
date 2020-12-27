using FluentValidation;
using Meal.API.Application.Commands.Meeting.CreateMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meal.API.Application.Validations
{
    public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
    {
        public CreateMeetingCommandValidator()
        {
            RuleFor(cmd => cmd.Name).NotEmpty().WithMessage("会议名称不能为空");

            RuleFor(cmd => cmd.Address).NotEmpty().WithMessage("会议地址不能为空");

            RuleFor(cmd => cmd.StartDate)
                .LessThanOrEqualTo(cmd => cmd.EndDate)
                .WithMessage("会议开始日期不能晚于结束日期")
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("会议开始日期已过期");
        }
    }
}
