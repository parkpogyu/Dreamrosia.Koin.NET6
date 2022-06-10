using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Constants.Coin;
using System;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class AssetProfile : Profile
    {
        public AssetProfile()
        {
            CreateMap<Order, AssetTrackingDto>().ForMember(d => d.IsOrder, o => o.MapFrom(s => true))
                                                .ForMember(d => d.uuid, o => o.MapFrom(s => s.Id))
                                                .ForMember(d => d.paid_fee, o => o.MapFrom(s => Convert.ToDouble(s.paid_fee)))
                                                .ForMember(d => d.price, o => o.MapFrom(s => Convert.ToDouble(s.price)))
                                                .ForMember(d => d.volume, o => o.MapFrom(s => Convert.ToDouble(s.volume)))
                                                .ForMember(d => d.executed_volume, o => o.MapFrom(s => Convert.ToDouble(s.executed_volume)))
                                                .ForMember(d => d.done_at, o => o.MapFrom(s => s.created_at));
            CreateMap<Transfer, AssetTrackingDto>().ForMember(d => d.IsOrder, o => o.MapFrom(s => false))
                                                   .ForMember(d => d.uuid, o => o.MapFrom(s => s.Id))
                                                   .ForMember(d => d.market, o => o.MapFrom(s => $"{Currency.Unit.KRW}-{s.code}"))
                                                   .ForMember(d => d.TransferState, o => o.MapFrom(s => s.state));
        }
    }
}