using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "현재가정보")]
    public class TickerDto
    {
        public string market { get; set; }
        public double opening_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public double trade_price { get; set; }
        public double signed_change_rate { get; set; }

        public static string GetPriceText(double price)
        {
            if (0 < price && price < 1)
            {
                return $"{price:N4}";
            }
            else if (1 < price && price < 100)
            {
                return $"{price:N2}";
            }
            else
            {
                return $"{price:N0}";
            }
        }
    }
}