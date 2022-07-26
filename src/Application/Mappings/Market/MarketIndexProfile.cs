using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class MarketIndexProfile : Profile
    {
        public MarketIndexProfile()
        {
            CreateMap<MarketIndex, MarketIndexDto>().ReverseMap()
                                                    .ForMember(s => s.Id, opt => opt.Ignore());

            CreateMap<MarketIndexDto, CandleDto>().ForMember(d => d.candle_date_time_utc, o => o.MapFrom(s => s.candleDateTimeUtc))
                                                  .ForMember(d => d.opening_price, o => o.MapFrom(s => s.openingPrice))
                                                  .ForMember(d => d.high_price, o => o.MapFrom(s => s.highPrice))
                                                  .ForMember(d => d.low_price, o => o.MapFrom(s => s.lowPrice))
                                                  .ForMember(d => d.trade_price, o => o.MapFrom(s => s.tradePrice));
        }
    }
}