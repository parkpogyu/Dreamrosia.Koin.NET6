using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Application.DTO
{
    public class BoasterDto : UserBriefDto
    {
        #region Membership
        public MembershipLevel MembershipLevel { get; set; }
        #endregion

        #region TradingTerms
        public TimeFrames TimeFrame { get; set; }
        #endregion
    }
}
