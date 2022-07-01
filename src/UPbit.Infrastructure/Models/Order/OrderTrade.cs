using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "체결정보")]
    public class OrderTrade
    {
        // 타입          필드               설명
        //-------------------------------------------------------------------------------------
        // String        trades.market     마켓의 유일 키 
        // String        trades.uuid       체결의 고유 아이디  
        // NumberString  trades.price      체결 가격 
        // NumberString  trades.volume     체결 양 
        // NumberString  trades.funds      체결된 총 가격    
        // String        trades.side       체결 종류 
        // DateString    trades.created_at 체결 시각 
        //-------------------------------------------------------------------------------------

        public string market { get; set; }
        public string uuid { get; set; }
        public decimal price { get; set; }
        public decimal volume { get; set; }
        public decimal funds { get; set; }
        public OrderSide Side { get; set; }
        public DateTime? created_at { get; set; }
    }
}
