using Dreamrosia.Koin.Domain.Contracts;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class MiningBotTicket : AuditableEntity<string>
    {
        public string UserId { get; set; }

        public IDomainUser User { get; set; }
        public MiningBot MiningBot { get; set; }
    }
}
