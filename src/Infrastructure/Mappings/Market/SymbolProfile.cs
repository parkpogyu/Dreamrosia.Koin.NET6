using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class SymbolProfile : Profile
    {
        public SymbolProfile()
        {
            CreateMap<UPbitModels.Symbol, SymbolDto>();
        }
    }
}