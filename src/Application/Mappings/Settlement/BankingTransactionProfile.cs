using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class BankingTransactionProfile : Profile
    {
        public BankingTransactionProfile()
        {
            CreateMap<BankingTransaction, BankingTransactionDto>().ReverseMap()
                                                                  .ForMember(p => p.Id, o => o.Ignore())
                                                                  .ForMember(p => p.UserCode, o => o.Ignore());
        }
    }
}