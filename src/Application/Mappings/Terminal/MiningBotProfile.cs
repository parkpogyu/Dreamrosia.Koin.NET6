using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;
using System;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class MiningBotProfile : Profile
    {
        public MiningBotProfile()
        {
            CreateMap<MiningBot, MiningBotDto>().ReverseMap()
                                                .ForMember(p => p.Ticket, o => o.Ignore())
                                                .ForMember(p => p.Touched, o => o.MapFrom(s => DateTime.Now));

            CreateMap<MiningBotTicket, MiningBotTicketDto>();
        }
    }
}