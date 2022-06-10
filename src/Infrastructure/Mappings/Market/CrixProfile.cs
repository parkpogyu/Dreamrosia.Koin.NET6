using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using System;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class CrixProfile : Profile
    {
        public CrixProfile()
        {
            CreateMap<UPbitModels.Crix, CrixDto>().ForMember(d => d.availableVolume, o => o.MapFrom(s => Convert.ToDouble(s.availableVolume)));
        }
    }
}