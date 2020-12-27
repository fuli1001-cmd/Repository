using Meal.API.Application.Commands.Meeting.CreateMeeting;
using Meal.API.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMeetingQueries _meetingQueries;

        public MeetingController(IMediator mediator, IMeetingQueries meetingQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _meetingQueries = meetingQueries ?? throw new ArgumentNullException(nameof(meetingQueries));
        }

        [HttpPost]
        public async Task<ActionResult<MeetingDto>> CreateMeetingAsync(CreateMeetingCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<List<MeetingDto>>> GetMeetingsAsync()
        {
            return await _meetingQueries.GetMeetingsAsync();
        }
    }
}
