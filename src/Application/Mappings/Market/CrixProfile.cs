using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class CrixProfile : Profile
    {
        public CrixProfile()
        {
            CreateMap<Crix, CrixDto>().ForMember(d => d.crix_code, o => o.MapFrom(s => s.Id))
                                      .ReverseMap()
                                      .ForMember(d => d.Id, o => o.MapFrom(s => s.crix_code));
        }
    }
}