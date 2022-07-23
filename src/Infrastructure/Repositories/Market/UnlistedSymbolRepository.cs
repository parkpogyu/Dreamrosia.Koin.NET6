using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class UnlistedSymbolRepository : IUnlistedSymbolRepository
    {
        private readonly IRepositoryAsync<UnlistedSymbol, string> _repository;

        public UnlistedSymbolRepository(IRepositoryAsync<UnlistedSymbol, string> repository)
        {
            _repository = repository;
        }
    }
}