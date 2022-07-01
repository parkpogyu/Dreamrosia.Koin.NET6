using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ForMember(d => d.uuid, o => o.MapFrom(s => s.Id))
                                        .ReverseMap()
                                        .ForMember(d => d.Id, o => o.MapFrom(s => s.uuid));

            CreateMap<Order, PaperOrderDto>().ForMember(d => d.uuid, o => o.MapFrom(s => s.Id));

            CreateMap<OrderDto, PaperOrderDto>();

            CreateMap<AssetTrackingDto, PaperOrderDto>().ForMember(d => d.created_at, o => o.MapFrom(s => s.done_at));
        }
    }
}