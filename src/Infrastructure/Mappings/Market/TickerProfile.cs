using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using System;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class TickerProfile : Profile
    {
        public TickerProfile()
        {
            CreateMap<UPbitModels.Ticker, TickerDto>().ForMember(d => d.signed_change_rate, o => o.MapFrom(s => s.signed_change_rate * StaticValue.Hundred));
            CreateMap<WsTicker.WsResponse, DelistingSymbolDto>().ForMember(d => d.NotifiedAt, o => o.MapFrom(s => DateTime.Now))
                                                                .ForMember(d => d.CloseAt,
                                                                           o => o.MapFrom(s => (DateTime?)(s.delisting_date == null ? null :
                                                                                                new DateTime(s.delisting_date.year,
                                                                                                             s.delisting_date.month,
                                                                                                             s.delisting_date.day))));
        }
    }
}