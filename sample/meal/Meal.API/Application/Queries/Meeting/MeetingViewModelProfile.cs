using AutoMapper;

namespace Meal.API.Application.Queries
{
    public class MeetingViewModelProfile : Profile
    {
        public MeetingViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.MeetingAggregate.Meeting, MeetingDto>();
        }
    }
}
