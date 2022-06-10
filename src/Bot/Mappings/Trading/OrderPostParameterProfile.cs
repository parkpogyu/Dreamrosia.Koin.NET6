using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;

namespace Dreamrosia.Koin.Bot.Mappings
{
    public class OrderPostParameterProfile : Profile
    {
        public OrderPostParameterProfile()
        {
            CreateMap<OrderPostParameterDto, ExOrderPost.ExParameter>();
        }
    }
}