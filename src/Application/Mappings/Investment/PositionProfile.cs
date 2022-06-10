using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<Position, PositionDto>().ReverseMap()
                                              .ForMember(s => s.Id, opt => opt.Ignore());

            CreateMap<PositionDto, PaperPositionDto>();

            CreateMap<PaperPositionDto, PaperPositionDto>();
        }
    }
}