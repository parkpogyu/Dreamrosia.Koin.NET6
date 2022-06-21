using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Enums;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    public class UserSummaryDto : UserDto
    {
        #region Membership
        public MembershipLevel MembershipLevel { get; set; }
        public long MaximumAsset { get; set; }
        public int DailyDeductionPoint { get; set; }
        #endregion

        #region TradingTerms
        public bool AutoTrading { get; set; }
        public TimeFrames TimeFrame { get; set; }
        #endregion

        #region MiningBot
        public bool IsAssignedBot { get; set; }
        #endregion

        #region Role
        public string RolesDescription { get; set; }
        #endregion

        [JsonIgnore]
        public bool ShowDetails { get; set; }
    }
}