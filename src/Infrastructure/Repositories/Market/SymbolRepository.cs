using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class SymbolRepository : ISymbolRepository
    {
        private readonly IRepositoryAsync<Symbol, string> _repository;

        public SymbolRepository(IRepositoryAsync<Symbol, string> repository)
        {
            _repository = repository;
        }
    }
}