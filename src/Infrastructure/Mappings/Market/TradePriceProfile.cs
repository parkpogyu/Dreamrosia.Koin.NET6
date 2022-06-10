using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class TradePriceProfile : Profile
    {
        public TradePriceProfile()
        {
            CreateMap<UPbitModels.Ticker, TickerDto>().ForMember(d => d.signed_change_rate, o => o.MapFrom(s => s.signed_change_rate * 100F));
        }
    }
}