using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class UPbitKeyRepository : IUPbitKeyRepository
    {
        private readonly IRepositoryAsync<UPbitKey, string> _repository;

        public UPbitKeyRepository(IRepositoryAsync<UPbitKey, string> repository)
        {
            _repository = repository;
        }
    }
}