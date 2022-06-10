using AutoMapper;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Infrastructure.Models.Identity;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, BlazorHeroRole>().ReverseMap();
        }
    }
}