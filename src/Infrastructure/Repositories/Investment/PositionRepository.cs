using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IRepositoryAsync<Position, int> _repository;

        public PositionRepository(IRepositoryAsync<Position, int> repository)
        {
            _repository = repository;
        }
    }
}