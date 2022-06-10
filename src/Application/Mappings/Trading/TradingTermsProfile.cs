using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class TradingTermsProfile : Profile
    {
        public TradingTermsProfile()
        {
            CreateMap<TradingTerms, TradingTermsDto>().ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
                                                      .ReverseMap()
                                                      .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId));

            CreateMap<TradingTermsDto, BackTestingRequestDto>();

            CreateMap<TradingTerms, TradingTermsExtensionDto>().ForMember(d => d.UserId, o => o.MapFrom(s => s.Id));
        }
    }
}