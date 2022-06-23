using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Infrastructure.Models.Identity;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserResponse, BlazorHeroUser>().ReverseMap();
            CreateMap<BlazorHeroUser, UserCardDto>();
            CreateMap<BlazorHeroUser, UserBriefDto>();
            CreateMap<BlazorHeroUser, UserDto>();
            CreateMap<BlazorHeroUser, BoasterDto>();
            CreateMap<BlazorHeroUser, FollowerDto>();
            CreateMap<BlazorHeroUser, UserFullInfoDto>();
        }
    }
}