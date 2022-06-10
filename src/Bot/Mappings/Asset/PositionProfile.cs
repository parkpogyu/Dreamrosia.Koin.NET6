using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Models;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Bot.Mappings
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<Position, PositionDto>().ForMember(d => d.UserId, o => o.MapFrom(s => Depot.UserId))
                                              .ForMember(d => d.trade_price, o => o.MapFrom(s => GetTadePrice($"{s.unit_currency}-{s.code}")));
        }

        private double GetTadePrice(string market)
        {
            return Depot.Tickers.ContainsKey(market) ? Depot.Tickers[market].trade_price : 0;
        }
    }
}