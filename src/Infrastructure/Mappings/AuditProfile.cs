using AutoMapper;
using Dreamrosia.Koin.Application.Responses.Audit;
using Dreamrosia.Koin.Infrastructure.Models.Audit;

namespace Dreamrosia.Koin.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}