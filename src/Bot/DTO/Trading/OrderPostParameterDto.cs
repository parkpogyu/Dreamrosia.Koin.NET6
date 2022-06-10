using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Bot.DTO
{
    public class OrderPostParameterDto
    {
        public string market { get; set; }

        public OrderSide side { get; set; }

        public double? volume { get; set; }

        public double? price { get; set; }

        public OrderType ord_type { get; set; }

        public string Remark { get; set; }

        public static double MinimumBidAmount => 10000;
        public static double MinimumOrderableAmount => 5000;
    }
}
