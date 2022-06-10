using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IRepositoryAsync<Order, string> _repository;

        public OrderRepository(IRepositoryAsync<Order, string> repository)
        {
            _repository = repository;
        }
    }
}