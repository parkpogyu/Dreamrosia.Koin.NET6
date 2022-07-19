using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class DelistingSymbolRepository : IDelistingSymbolRepository
    {
        private readonly IRepositoryAsync<DelistingSymbol, string> _repository;

        public DelistingSymbolRepository(IRepositoryAsync<DelistingSymbol, string> repository)
        {
            _repository = repository;
        }
    }
}