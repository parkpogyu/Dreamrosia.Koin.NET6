using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Constants.Application;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class SymbolProfile : Profile
    {
        public SymbolProfile()
        {
            CreateMap<Symbol, SymbolDto>().ForMember(d => d.market, o => o.MapFrom(s => s.Id))
                                          .ReverseMap()
                                          .ForMember(d => d.Id, o => o.MapFrom(s => s.market));

            CreateMap<CrixDto, SymbolDto>().ForMember(d => d.trade_price, o => o.MapFrom(s => s.price))
                                           .ForMember(d => d.marketCap, o => o.MapFrom(s => s.marketCap / StaticValue.HundredMillion))
                                           .ForMember(d => d.accTradePrice24h, o => o.MapFrom(s => s.accTradePrice24h / StaticValue.HundredMillion));

            CreateMap<SeasonSignalDto, SymbolDto>();

            CreateMap<DelistingSymbol, DelistingSymbolDto>().ForMember(d => d.market, o => o.MapFrom(s => s.Id))
                                                            .ReverseMap()
                                                            .ForMember(d => d.Id, o => o.MapFrom(s => s.market));
        }
    }
}