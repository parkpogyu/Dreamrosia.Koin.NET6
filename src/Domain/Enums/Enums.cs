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
}
