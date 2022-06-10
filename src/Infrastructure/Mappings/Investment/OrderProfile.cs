using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<UPbitModels.Order, OrderDto>().ReverseMap();
        }
    }
}