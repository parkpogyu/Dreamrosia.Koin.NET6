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

        // Device               Code    Type                    Range
        //-----------------------------------------------------------------
        // Extra small          xs      Small to large phone    < 600px
        // Small                sm      Small to medium tablet	600px >< 960px
        // Medium               md      Large tablet to laptop	960px >< 1280px
        // Large                lg      Desktop	                1280px >< 1920px
        // Extra Large          xl      HD and 4k	            1920px >< 2560px
        // Extra Extra Large    xx	    4k+ and ultra-wide	    >= 2560px*

        public static bool IsMatchMediumBreakPoints(double height)
        {
            return height > 768 ? true : false;
        }
    }
}
