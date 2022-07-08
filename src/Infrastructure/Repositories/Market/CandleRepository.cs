using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class CandleRepository : ICandleRepository
    {
        private readonly IRepositoryAsync<Candle, int> _repository;

        public CandleRepository(IRepositoryAsync<Candle, int> repository)
        {
            _repository = repository;
        }
    }
}