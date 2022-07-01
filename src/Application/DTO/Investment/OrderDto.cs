using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "주문정보")]
    public class OrderDto
    {
        public string UserId { get; set; }

        public string uuid { get; set; }
        public OrderSide side { get; set; }
        public OrderType ord_type { get; set; }
        public decimal? price { get; set; }
        public OrderState state { get; set; }
        private string _market { get; set; }
        public string market
        {
            get => _market;
            set
            {
                _market = value;

                unit_currency = Symbol.GetUnitCurrency(_market);
                code = Symbol.GetCode(market);
            }
        }
        public DateTime created_at { get; set; }
        public decimal? volume { get; set; }
        public decimal? remaining_volume { get; set; }
        public decimal reserved_fee { get; set; }
        public decimal remaining_fee { get; set; }
        public decimal paid_fee { get; set; }
        public decimal locked { get; set; }
        public decimal executed_volume { get; set; }
        public int trades_count { get; set; }

        // 2019.-01-27 이전 데이터 자료 없는 경우 발생
        public double avg_price { get; set; }
        public double exec_amount { get; set; }

        public string unit_currency { get; private set; }
        public string code { get; private set; }

        [JsonIgnore]
        public OrderState ConvertedState => (state == OrderState.cancel && trades_count > 0) ? OrderState.done : state;

        public double PnL { get; set; }
        public float PnLRat { get; set; }
    }
}