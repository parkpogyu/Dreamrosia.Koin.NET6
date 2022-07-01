using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Constants.Coin;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class AssetProfile : Profile
    {
        public AssetProfile()
        {
            CreateMap<Order, AssetTrackingDto>().ForMember(d => d.IsOrder, o => o.MapFrom(s => true))
                                                .ForMember(d => d.uuid, o => o.MapFrom(s => s.Id))
                                                .ForMember(d => d.done_at, o => o.MapFrom(s => s.created_at));
            CreateMap<Transfer, AssetTrackingDto>().ForMember(d => d.IsOrder, o => o.MapFrom(s => false))
                                                   .ForMember(d => d.uuid, o => o.MapFrom(s => s.Id))
                                                   .ForMember(d => d.market, o => o.MapFrom(s => $"{Currency.Unit.KRW}-{s.code}"))
                                                   .ForMember(d => d.TransferState, o => o.MapFrom(s => s.state));
        }
    }
}