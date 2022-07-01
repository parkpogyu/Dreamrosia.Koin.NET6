using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{

    [Display(Name = "코인정보")]
    public class Coin : Symbol
    {
        /// <summary>
        /// 시가총액 정보
        /// </summary>
        public Crix Crix { get; set; }

        /// <summary>
        /// 코인상태 
        /// </summary>
        public CoinStatus CoinStatus { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        public Ticker Ticker { get; set; }

        /// <summary>
        /// 호가
        /// </summary>
        public OrderBook OrderBook { get; set; }

        public Coin()
        {
            Crix = new Crix();
            CoinStatus = new CoinStatus();
            Ticker = new Ticker();
            OrderBook = new OrderBook();
        }
    }
}
