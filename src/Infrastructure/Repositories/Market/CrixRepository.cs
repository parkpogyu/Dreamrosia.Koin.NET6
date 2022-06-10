using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class CrixRepository : ICrixRepository
    {
        private readonly IRepositoryAsync<Crix, string> _repository;

        public CrixRepository(IRepositoryAsync<Crix, string> repository)
        {
            _repository = repository;
        }
    }
}