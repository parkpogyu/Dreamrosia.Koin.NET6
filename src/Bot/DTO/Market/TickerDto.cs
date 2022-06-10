using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "현재가정보")]
    public class TickerDto
    {
        /// <summary>
        /// 마켓코드 
        /// </summary>
        public string market { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        public double trade_price { get; set; }
    }
}