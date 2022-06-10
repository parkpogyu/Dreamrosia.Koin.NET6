using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "현재가정보")]
    public class TickerDto
    {
        /// <summary>
        /// 마켓코드 
        /// </summary>
        public string market { get; set; }

        /// <summary>
        /// 시가
        /// </summary>
        public double opening_price { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        public double high_price { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        public double low_price { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        public double trade_price { get; set; }

        /// <summary>
        /// 등락률(%)
        /// </summary>
        public double signed_change_rate { get; set; }

        public static string GetPriceText(double price)
        {
            if (price == 0)
            {
                return $"{price:N0}";
            }
            else if (0 < price && price < 1)
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