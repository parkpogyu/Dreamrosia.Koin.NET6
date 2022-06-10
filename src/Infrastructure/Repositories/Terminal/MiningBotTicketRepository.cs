using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class MiningBotTicketRepository : IMiningBotTicketRepository
    {
        private readonly IRepositoryAsync<MiningBotTicket, string> _repository;

        public MiningBotTicketRepository(IRepositoryAsync<MiningBotTicket, string> repository)
        {
            _repository = repository;
        }
    }
}