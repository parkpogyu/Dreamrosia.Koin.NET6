using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Order : AuditableEntity<string>
    {
        /// 주문번호
        /// Id <-> uuid

        public string UserId { get; set; }

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

        public double avg_price { get; set; }
        public double exec_amount { get; set; }

        public IDomainUser User { get; set; }
    }
}
