using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "주문정보")]
    public class Order
    {
        // 타입          필드               설명
        //-------------------------------------------------------------------------------------
        // String        uuid              주문의 고유 아이디  
        // String        side              주문 종류
        // String        ord_type          주문 방식
        // NumberString  price             주문 당시 화폐 가격 
        // String        state             주문 상태
        // String        market            마켓의 유일키 
        // DateString    created_at        주문 생성 시간 
        // NumberString  volume            사용자가 입력한 주문 양 
        // NumberString  remaining_volume  체결 후 남은 주문 양    
        // NumberString  reserved_fee      수수료로 예약된 비용 
        // NumberString  remaining_fee     남은 수수료 
        // NumberString  paid_fee          사용된 수수료 
        // NumberString  locked            거래에 사용중인 비용 
        // NumberString  executed_volume   체결된 양   
        // Integer       trade_count       해당 주문에 걸린 체결 수 
        //-------------------------------------------------------------------------------------

        public string uuid { get; set; }
        public OrderSide side { get; set; }
        public OrderType ord_type { get; set; }
        public decimal? price { get; set; }
        public OrderState state { get; set; }
        public string market { get; set; }
        public DateTime created_at { get; set; }
        public decimal? volume { get; set; }
        public decimal? remaining_volume { get; set; }
        public decimal reserved_fee { get; set; }
        public decimal remaining_fee { get; set; }
        public decimal paid_fee { get; set; }
        public decimal locked { get; set; }
        public decimal executed_volume { get; set; }
        public int trades_count { get; set; }

        #region Appended
        /// <summary>
        /// 평균단가
        /// </summary>
        public double avg_price { get; set; }

        /// <summary>
        /// 체결금액
        /// </summary>
        public double exec_amount { get; set; }
        #endregion
    }
}
