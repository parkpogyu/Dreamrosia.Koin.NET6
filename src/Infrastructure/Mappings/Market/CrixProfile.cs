using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class CrixProfile : Profile
    {
        public CrixProfile()
        {
            CreateMap<UPbitModels.Crix, CrixDto>();
        }
    }
}