using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Infrastructure.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly IRepositoryAsync<Membership, int> _repository;

        public MembershipRepository(IRepositoryAsync<Membership, int> repository)
        {
            _repository = repository;
        }
    }
}