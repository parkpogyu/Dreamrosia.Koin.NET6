using Dreamrosia.Koin.Domain.Enums;
using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class SubscriptionDto
    {
        public string UserId { get; set; }

        #region Membership
        public MembershipLevel Level { get; set; }
        public long MaximumAsset { get; set; }
        public float CommissionRate { get; set; }
        public int DailyDeductionPoint { get; set; }
        public DateTime  LastCreatedOn { get; set; }
        #endregion

        public bool GoBoast { get; set; }
        public UserDto Recommender { get; set; }
    }
}