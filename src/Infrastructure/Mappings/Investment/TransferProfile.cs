using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class TransferProfile : Profile
    {
        public TransferProfile()
        {
            CreateMap<UPbitModels.Transfer, TransferDto>().ReverseMap();
        }
    }
}