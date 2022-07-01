using Dreamrosia.Koin.Domain.Enums;
using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class MembershipDto
    {
        public string UserId { get; set; }

        public MembershipLevel Level { get; set; }
        public float MaximumAsset { get; set; }
        public float CommissionRate { get; set; }
        public int DailyDeductionPoint { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
