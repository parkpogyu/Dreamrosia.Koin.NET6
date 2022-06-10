using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class SeasonSignalProfile : Profile
    {
        public SeasonSignalProfile()
        {
            CreateMap<Symbol, SeasonSignalDto>().ForMember(d => d.market, o => o.MapFrom(s => s.Id));

            CreateMap<SeasonSignal, SeasonSignalDto>().ReverseMap()
                                                      .ForMember(d => d.Id, o => o.Ignore());
        }
    }
}