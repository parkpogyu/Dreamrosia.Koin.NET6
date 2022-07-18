using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Domain.Enums;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Subscription : AuditableEntity<string>
    {
        // Id <-> UserId
        #region Membership
        public MembershipLevel Level { get; set; }
        public long MaximumAsset { get; set; }
        public float CommissionRate { get; set; }
        public int DailyDeductionPoint { get; set; }
        #endregion

        public bool GoBoast { get; set; }
        public string RecommenderId { get; set; }

        public IDomainUser User { get; set; }
        public IDomainUser Recommender { get; set; }
    }
}
