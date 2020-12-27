using Meal.API.Application.Queries;
using MediatR;
using System;

namespace Meal.API.Application.Commands.Meeting.CreateMeeting
{
    public class CreateMeetingCommand : IRequest<MeetingDto>
    {
        public string Name { get; }

        public string Address { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public CreateMeetingCommand(string name, string address, DateTime startDate, DateTime endDate)
        {
            Name = name;
            Address = address;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
