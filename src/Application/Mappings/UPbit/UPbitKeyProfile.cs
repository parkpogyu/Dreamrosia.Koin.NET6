using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class UPbitKeyProfile : Profile
    {
        public UPbitKeyProfile()
        {
            CreateMap<UPbitKey, UPbitKeyDto>().ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
                                              .ReverseMap()
                                              .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId));

            CreateMap<UPbitKeyDto, UPbitKeyDto>();
            CreateMap<UPbitKeyDto, UPbitKeyTestDto>().ReverseMap();

            CreateMap<UPbitKeyTestDto, UPbitKeyTestDto>();
        }
    }
}