using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "주문정보")]
    public class Order
    {
        [Display(Name = "마켓코드", GroupName = "주문정보")]
        public string market { get; set; }

        [Display(Name = "주문종류", GroupName = "주문정보")]
        public OrderSide side { get; set; }

        [Display(Name = "주문방식", GroupName = "주문정보")]
        public OrderType ord_type { get; set; }

        [Display(Name = "주문가격", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? price { get; set; }

        [Display(Name = "주문수량", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double? volume { get; set; }

        [Display(Name = "주문금액", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? amount { get; set; }

        [Display(Name = "주문상태", GroupName = "주문정보")]
        public OrderState? state { get; set; }

        [Display(Name = "주문번호", GroupName = "주문정보")]
        public string uuid { get; set; }

        [Display(Name = "체결수량", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double? executed_volume { get; set; }

        [Display(Name = "잔여수량", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double? remaining_volume { get; set; }

        [Display(Name = "예약수수료", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double reserved_fee { get; set; }

        [Display(Name = "지불수수료", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? paid_fee { get; set; }

        [Display(Name = "잔여수수료", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double remaining_fee { get; set; }

        [Display(Name = "사용중인 비용", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double locked { get; set; }

        [Display(Name = "체결횟수", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? trades_count { get; set; }

        #region Appended
        [Display(Name = "평균단가", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double avg_price { get; set; }

        [Display(Name = "체결금액", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double exec_amount { get; set; }
        #endregion

        [Display(Name = "주문시간", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime created_at { get; set; }

        public static float Fee => 0.0005F;

        [Display(Name = "최소주문가능금액", GroupName = "주문정보")]
        public static int MinimumOrderableAmount = 10000;
    }
}
