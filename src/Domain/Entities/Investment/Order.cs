using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Order : AuditableEntity<string>
    {
        /// 주문번호
        /// Id <-> uuid

        //[Required]
        //[StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// 마켓코드
        /// </summary>
        //[Required]
        //[StringLength(20)]
        public string market { get; set; }

        /// <summary>
        /// 주문종류
        /// </summary>
        //[Required]
        public OrderSide side { get; set; }

        /// <summary>
        /// 주문방식
        /// </summary>
        //[Required]
        public OrderType ord_type { get; set; }

        /// <summary>
        /// 주문가격
        /// </summary>
        public double? price { get; set; }

        /// <summary>
        /// 주문수량
        /// </summary>
        public double? volume { get; set; }

        /// <summary>
        /// 주문금액
        /// </summary>
        public double? amount { get; set; }

        /// <summary>
        /// 주문상태
        /// </summary>
        public OrderState? state { get; set; }

        /// <summary>
        /// 체결수량
        /// </summary>
        public double? executed_volume { get; set; }

        /// <summary>
        /// 잔여수량
        /// </summary>
        public double? remaining_volume { get; set; }

        /// <summary>
        /// 예약 수수료
        /// </summary>
        public double reserved_fee { get; set; }

        /// <summary>
        /// 지불 수수료
        /// </summary>
        public double? paid_fee { get; set; }

        /// <summary>
        /// 잔여 수수료
        /// </summary>
        public double remaining_fee { get; set; }

        /// <summary>
        /// 묶여있는 비용
        /// </summary>
        public double locked { get; set; }

        /// <summary>
        /// 체결건수
        /// </summary>
        public int? trades_count { get; set; }

        /// <summary>
        /// 평균단가
        /// </summary>
        public double avg_price { get; set; }

        /// <summary>
        /// 체결금액
        /// </summary>
        public double exec_amount { get; set; }

        /// <summary>
        /// 주문일시
        /// </summary>
        //[Required]
        public DateTime created_at { get; set; }

        public IDomainUser User { get; set; }
    }
}
