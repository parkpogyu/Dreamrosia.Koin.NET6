using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class PointRepository : IPointRepository
    {
        private readonly IRepositoryAsync<Point, int> _repository;

        public PointRepository(IRepositoryAsync<Point, int> repository)
        {
            _repository = repository;
        }
    }
}