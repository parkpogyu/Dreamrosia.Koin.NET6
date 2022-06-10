using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class SeasonSignalRepository : ISeasonSignalRepository
    {
        private readonly IRepositoryAsync<SeasonSignal, int> _repository;

        public SeasonSignalRepository(IRepositoryAsync<SeasonSignal, int> repository)
        {
            _repository = repository;
        }
    }
}