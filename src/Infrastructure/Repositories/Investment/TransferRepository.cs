using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly IRepositoryAsync<Transfer, string> _repository;

        public TransferRepository(IRepositoryAsync<Transfer, string> repository)
        {
            _repository = repository;
        }
    }
}