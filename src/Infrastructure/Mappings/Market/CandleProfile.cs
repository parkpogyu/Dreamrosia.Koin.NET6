using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class CandleProfile : Profile
    {
        public CandleProfile()
        {
            CreateMap<UPbitModels.Candle, CandleDto>();
        }
    }
}