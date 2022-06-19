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

        //[Description("크리스탈")]
        //Crystal,

        //[Description("루비")]
        //Ruby,

        //[Description("사파이어")]
        //Sappire,

        //[Description("에메랄드")]
        //Emerald,

        //[Description("다이아몬드")]
        //Diamond,
    }

    public enum PointType
    {
        [Description("환불")]
        Refund,

        [Description("충전")]
        Charging,

        [Description("사용")]
        Redeem,

        [Description("쿠폰")]
        Coupon,
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
