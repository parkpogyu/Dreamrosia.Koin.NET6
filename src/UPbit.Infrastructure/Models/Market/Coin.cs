using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{

    [Display(Name = "코인정보")]
    public class Coin : Symbol
    {
        [Display(Name = "시가총액")]
        public Crix Crix { get; set; }

        [Display(Name = "코인상태")]
        public CoinStatus CoinStatus { get; set; }

        [Display(Name = "현재가")]
        public Ticker Ticker { get; set; }

        [Display(Name = "호가")]
        public OrderBook OrderBook { get; set; }

        public Coin()
        {
            Crix = new Crix();
            CoinStatus = new CoinStatus();
            Ticker = new Ticker();
            OrderBook = new OrderBook();
        }

        public static string KRW => "KRW";
        public static string BTC => "BTC";
        public static string USDT => "USDT";
    }
}
