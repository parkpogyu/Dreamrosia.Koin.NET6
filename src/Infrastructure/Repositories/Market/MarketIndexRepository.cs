using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class MarketIndexRepository : IMarketIndexRepository
    {
        private readonly IRepositoryAsync<MarketIndex, int> _repository;

        public MarketIndexRepository(IRepositoryAsync<MarketIndex, int> repository)
        {
            _repository = repository;
        }
    }
}