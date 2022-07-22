namespace Dreamrosia.Koin.Bot.Constants
{
    public static class TradingConstants
    {
        /// <summary>
        /// 절사단위
        /// </summary>
        public static int RoundDown => 1000;

        /// <summary>
        /// 업비트 최소 주문금액
        /// </summary>
        public static int MinOrderableAmount => 5000;

        /// <summary>
        /// 최소 주문금액
        /// </summary>
        public static int MinBidAmount => 10000;

        /// <summary>
        /// 업비트 최대 주문금액: 1,000,000,000
        /// </summary>
        public static int MaxBidAmount => 1000000000;

    }
}
