using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Bot.Mappings
{
    public class UPbitKeyProfile : Profile
    {
        public UPbitKeyProfile()
        {
            CreateMap<UPbitKey, UPbitKeyDto>().ReverseMap();
        }
    }
}