namespace Dreamrosia.Koin.Shared.Constants.Application
{
    public static class StaticValue
    {
        public static class TradingTerms
        {
            public static int MaximumAsset4Free => 1500000;
            public static int MaximumAsset4Basic => 15000000;
            public static int MinimumAsset4Advanced => 20000000;
            public static int MinimumAmount => 10000;
            public static int MaximumAmount => 1000000000;
            public static long MaximumAsset => 10000000000;
            public static int RoundDown => 1000;
        }

        public static class Fees
        {
            public static float CommissionRate => 3;

            public static float Rate4KRW => 0.0005F;

            public static float Rate4BTC => 0.0025F;

            public static float Rate4USDT => 0.0025F;
        }

        public static class ChargingPoint
        {
            public static int Free => 0;
            public static int Basic => 990;
            public static int Divider => 5000000;
        }

        public static int Hundred => 100;
        public static int Thousand => 1000;
        public static int HundredMillion => 100000000;
        public static int Billion => 1000000000;
    }
}