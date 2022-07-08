using AutoMapper;
using Dreamrosia.Koin.Domain.Entities;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class MarketIndexProfile : Profile
    {
        public MarketIndexProfile()
        {
            CreateMap<UPbitModels.MarketIndex, MarketIndex>().ForMember(d => d.Id, o => o.Ignore())
                                                             .ForMember(d => d.candleDateTimeUtc, o => o.MapFrom(s => s.candleDateTimeUtc.ToUniversalTime().Date));
        }
    }
}