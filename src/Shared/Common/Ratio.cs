namespace Dreamrosia.Koin.Shared.Common
{
    public class Ratio
    {
        public static double ToRatio(double numerator, double denominator)
        {
            return numerator / denominator;
        }

        public static double ToSignedPercentage(double numerator, double denominator)
        {
            return (ToRatio(numerator, denominator) - 1) * Hundreadth;
        }

        public static double ToPercentage(double numerator, double denominator)
        {
            return ToRatio(numerator, denominator) * Hundreadth;
        }

        public static double Hundreadth => 100D;
    }

}
