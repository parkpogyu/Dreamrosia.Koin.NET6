using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IRepositoryAsync<Subscription, string> _repository;

        public SubscriptionRepository(IRepositoryAsync<Subscription, string> repository)
        {
            _repository = repository;
        }
    }
}