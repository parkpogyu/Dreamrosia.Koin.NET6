using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<UPbitModels.Position, PositionDto>();
        }
    }
}