using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Models;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Bot.Mappings
{
    public class TransferProfile : Profile
    {
        public TransferProfile()
        {
            CreateMap<Transfer, TransferDto>().ForMember(d => d.UserId, o => o.MapFrom(s => Depot.UserId));
        }
    }
}