using Dreamrosia.Koin.Domain.Contracts;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Subscription : AuditableEntity<string>
    {
        // Id <-> UserId

        public bool GoBoast { get; set; }

        public string RecommenderId { get; set; }

        public IDomainUser User { get; set; }
    }
}
