using Dreamrosia.Koin.Application.DTO;

namespace Dreamrosia.Koin.Application.Extensions
{
    public static class MembershipExtensions
    {
        public static int GetDailyDeductionPoint(this MembershipDto membership)
        {
            return (int)(((membership.MaximumAsset * membership.CommissionRate / 100F) / 365F) / 10F) * 10;
        }
    }
}
