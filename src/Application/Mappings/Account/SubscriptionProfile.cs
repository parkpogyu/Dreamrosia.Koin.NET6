using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, SubscriptionDto>().ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
                                                      .ReverseMap()
                                                      .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId));
        }
    }
}