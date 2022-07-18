using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            CreateMap<Membership, MembershipDto>().ReverseMap()
                                                  .ForMember(d => d.Id, o => o.Ignore())
                                                  .ForMember(d => d.CreatedOn, o => o.Ignore());

            CreateMap<MembershipDto, MembershipDto>();
            CreateMap<SubscriptionDto, MembershipDto>().ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.LastCreatedOn))
                                                       .ReverseMap()
                                                       .ForMember(d => d.LastCreatedOn, o => o.MapFrom(s => s.CreatedOn));
        }
    }
}