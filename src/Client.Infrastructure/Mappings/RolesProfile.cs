using AutoMapper;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;

namespace Dreamrosia.Koin.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}