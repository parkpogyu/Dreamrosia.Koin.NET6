using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class TransferProfile : Profile
    {
        public TransferProfile()
        {
            CreateMap<Transfer, TransferDto>().ForMember(d => d.uuid, o => o.MapFrom(s => s.Id))
                                              .ReverseMap()
                                              .ForMember(d => d.Id, o => o.MapFrom(s => s.uuid));
        }
    }
}