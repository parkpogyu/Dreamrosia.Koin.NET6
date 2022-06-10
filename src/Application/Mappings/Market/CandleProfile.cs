using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class CandleProfile : Profile
    {
        public CandleProfile()
        {
            CreateMap<Candle, CandleDto>().ForMember(d => d.candle_acc_trade_price, o => o.MapFrom(s => s.candle_acc_trade_price / 100000000))
                                          .ReverseMap()
                                          .ForMember(s => s.Id, opt => opt.Ignore());

            CreateMap<CandleDto, CandleExtensionDto>();
        }
    }
}