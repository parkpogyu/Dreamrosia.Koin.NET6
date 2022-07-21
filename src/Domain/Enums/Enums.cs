using System.ComponentModel;

namespace Dreamrosia.Koin.Domain.Enums
{
    public enum MembershipLevel
    {
        [Description("무료")]
        Free,

        [Description("일반")]
        Basic,

        [Description("고급")]
        Advanced,
    }

    public enum PointType
    {
        [Description("충전")]
        Charging,

        [Description("쿠폰")]
        Coupon,

        [Description("인센티브")]
        Incentive,

        [Description("사용")]
        Deduct,

        [Description("환불")]
        Refund,
    }

    public enum DateRangeTerms
    {
        [Description("일주")]
        _1W,

        [Description("한달")]
        _1M,

        [Description("세달")]
        _3M,

        [Description("여섯달")]
        _6M,

        [Description("일년")]
        _1Y,

        [Description("올해")]
        _YTD,

        [Description("모두")]
        _All
    }
}
