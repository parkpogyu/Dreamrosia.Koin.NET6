using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using System;
using System.Globalization;
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

            CreateMap<UPbitModels.Ticker, CandleDto>().ForMember(d => d.candle_date_time_utc, o => o.MapFrom(s => ConvertStringDate(s.trade_date)))
                                                      .ForMember(d => d.candle_date_time_kst, o => o.MapFrom(s => ConvertStringDate(s.trade_date).AddHours(9)))
                                                      .ForMember(d => d.candle_acc_trade_price, o => o.MapFrom(s => s.acc_trade_price))
                                                      .ForMember(d => d.candle_acc_trade_volume, o => o.MapFrom(s => s.acc_trade_volume));
        }

        private DateTime ConvertStringDate(string date)
        {

            DateTime convert;

            DateTime.TryParseExact(date, "yyyyMMdd", null, DateTimeStyles.AssumeLocal, out convert);

            return convert;
        }
    }
}