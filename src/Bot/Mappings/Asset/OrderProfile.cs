using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Models;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Bot.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ForMember(d => d.UserId, o => o.MapFrom(s => Depot.UserId));

            CreateMap<OrderPostParameterDto, ExOrderPost.ExParameter>();
        }
    }
}