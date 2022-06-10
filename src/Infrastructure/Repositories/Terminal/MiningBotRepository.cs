using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class MiningBotRepository : IMiningBotRepository
    {
        private readonly IRepositoryAsync<MiningBot, string> _repository;

        public MiningBotRepository(IRepositoryAsync<MiningBot, string> repository)
        {
            _repository = repository;
        }
    }
}