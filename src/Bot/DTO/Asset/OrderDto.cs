using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "주문정보")]
    public class OrderDto
    {
        public string UserId { get; set; }

        public string market { get; set; }

        public OrderSide side { get; set; }

        public OrderType ord_type { get; set; }

        public double? price { get; set; }

        public double? volume { get; set; }

        public double? amount { get; set; }

        public OrderState? state { get; set; }

        public string uuid { get; set; }

        public double? executed_volume { get; set; }

        public double? remaining_volume { get; set; }

        public double reserved_fee { get; set; }

        public double? paid_fee { get; set; }

        public double remaining_fee { get; set; }

        public double locked { get; set; }

        public int? trades_count { get; set; }

        public double avg_price { get; set; }

        public double exec_amount { get; set; }

        public DateTime created_at { get; set; }
    }
}