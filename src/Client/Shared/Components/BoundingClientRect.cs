namespace Dreamrosia.Koin.Client.Shared.Components
{
    public class BoundingClientRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }

        public static bool IsMatchMimimumHeight(double height)
        {
            // SmallUp = "(min-width: 576px)";
            // MediumUp = "(min-width: 768px)";
            // LargeUp = "(min-width: 992px)";
            // XLargeUp = "(min-width: 1200px)";
            // XSmallDown = "(max-width: 575.98px)";
            // SmallDown = "(max-width: 767.98px)";
            // MediumDown = "(max-width: 991.98px)";
            // LargeDown = "(max-width: 1199.98px)";
            // OnlySmall => Between("(max-width: 767.98px)", "(min-width: 576px)");
            // OnlyMedium => Between("(max-width: 991.98px)", "(min-width: 768px)");
            // OnlyLarge => Between("(max-width: 1199.98px)", "(min-width: 992px)");

            return height > 768 ? true : false;
        }
    }
}
