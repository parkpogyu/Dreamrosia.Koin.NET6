using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class TickerProfile : Profile
    {
        public TickerProfile()
        {
            CreateMap<Ticker, TickerDto>().ForMember(d => d.market, o => o.MapFrom(s => s.code));
        }
    }
}