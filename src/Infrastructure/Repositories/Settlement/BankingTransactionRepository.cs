using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class BankingTransactionRepository : IBankingTransactionRepository
    {
        private readonly IRepositoryAsync<BankingTransaction, int> _repository;

        public BankingTransactionRepository(IRepositoryAsync<BankingTransaction, int> repository)
        {
            _repository = repository;
        }
    }
}