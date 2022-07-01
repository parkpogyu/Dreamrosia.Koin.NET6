using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Domain.Enums;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Membership : AuditableEntity<int>
    {
        public string UserId { get; set; }

        public MembershipLevel Level { get; set; }
        public long MaximumAsset { get; set; }
        public float CommissionRate { get; set; }
        public int DailyDeductionPoint { get; set; }

        public IDomainUser User { get; set; }
    }
}
