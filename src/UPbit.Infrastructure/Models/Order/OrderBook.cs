using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "호가정보")]
    public class OrderBook
    {

        // 타입          필드            설명
        //-------------------------------------------------------------------------------------
        // String       market          마켓 코드 
        // Long         timestamp       호가 생성 시각    
        // Double       total_ask_size  호가 매도 총 잔량
        // Double       total_bid_size  호가 매수 총 잔량  
        // List[Object] orderbook_units 호가 List of Objects
        //-------------------------------------------------------------------------------------

        public string market { get; set; }
        public long timestamp { get; set; }
        public double total_ask_size { get; set; }
        public double total_bid_size { get; set; }
        public List<OrderBookUnit> orderbook_units { get; set; }

        public class OrderBookUnit
        {
            // 타입   필드       설명
            //-------------------------------------------------------------------------------------
            // Double ask_price 매도호가    
            // Double bid_price 매수호가 
            // Double ask_size  매도 잔량 
            // Double bid_size  매수 잔량
            //-------------------------------------------------------------------------------------
            public double ask_price { get; set; }
            public double bid_price { get; set; }
            public double ask_size { get; set; }
            public double bid_size { get; set; }
        }

        public class Unit
        {
            public double? bid_size { get; set; }
            public double price { get; set; }
            public double? ask_size { get; set; }

            public OrderSide Side => bid_size != null ? OrderSide.bid : OrderSide.ask;
        }
    }
}
