using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class TradingTermsRepository : ITradingTermsRepository
    {
        private readonly IRepositoryAsync<TradingTerms, string> _repository;

        public TradingTermsRepository(IRepositoryAsync<TradingTerms, string> repository)
        {
            _repository = repository;
        }
    }
}