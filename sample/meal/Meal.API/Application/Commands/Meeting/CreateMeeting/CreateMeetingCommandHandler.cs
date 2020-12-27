using AutoMapper;
using Meal.API.Application.Queries;
using Meal.Domain.AggregatesModel.MeetingAggregate;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meal.API.Application.Commands.Meeting.CreateMeeting
{
    public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, MeetingDto>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;

        public CreateMeetingCommandHandler(IMeetingRepository meetingRepository, IMapper mapper)
        {
            _meetingRepository = meetingRepository ?? throw new ArgumentNullException(nameof(meetingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MeetingDto> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            var meeting = new Domain.AggregatesModel.MeetingAggregate.Meeting(request.Name, request.Address, request.StartDate, request.EndDate);

            _meetingRepository.Add(meeting);
            await _meetingRepository.UnitOfWork.SaveAsync(cancellationToken);

            return _mapper.Map<MeetingDto>(meeting);
        }
    }
}
