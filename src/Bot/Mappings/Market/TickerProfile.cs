using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Bot.Mappings
{
    public class TickerProfile : Profile
    {
        public TickerProfile()
        {
            CreateMap<Ticker, TickerDto>();
            CreateMap<Crix, TickerDto>().ForMember(d => d.market, o => o.MapFrom(s => $"{s.unit_currency}-{s.code}"))
                                        .ForMember(d => d.trade_price, o => o.MapFrom(s => s.price));
        }
    }
}