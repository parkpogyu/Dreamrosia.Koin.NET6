using AutoMapper;
using Dreamrosia.Koin.Application.DTO;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<BoasterDto, FollowerDto>();
        }
    }
}