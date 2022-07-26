using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class MarketIndexProfile : Profile
    {
        public MarketIndexProfile()
        {
            CreateMap<UPbitModels.MarketIndex, MarketIndexDto>().ForMember(d => d.candleDateTimeUtc, o => o.MapFrom(s => s.candleDateTimeUtc.ToUniversalTime().Date));
        }
    }
}