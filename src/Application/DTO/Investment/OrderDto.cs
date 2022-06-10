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

        public string market { get; set; }

        public OrderSide side { get; set; }

        public OrderType ord_type { get; set; }

        public double? price { get; set; }

        public double? volume { get; set; }

        public double? amount { get; set; }

        public OrderState? state { get; set; }

        [JsonIgnore]
        public OrderState? ConvertedState => (state == OrderState.cancel && trades_count > 0) ? OrderState.done : state;

        public string uuid { get; set; }

        public double? executed_volume { get; set; }

        public double? remaining_volume { get; set; }

        public double reserved_fee { get; set; }

        public double? paid_fee { get; set; }

        public double remaining_fee { get; set; }

        public double locked { get; set; }

        public int? trades_count { get; set; }

        // 2019.-01-27 이전 데이터 자료 없는 경우 발생
        private double _avg_price { get; set; }
        public double avg_price
        {
            get
            {
                return _avg_price == 0 ? Convert.ToDouble(price) : _avg_price;
            }

            set
            {
                _avg_price = value;
            }
        }

        private double _exec_amount { get; set; }
        public double exec_amount
        {
            get
            {
                return _exec_amount == 0 ? Convert.ToDouble(price) * Convert.ToDouble(executed_volume) : _exec_amount;
            }

            set
            {
                _exec_amount = value;
            }
        }

        public DateTime created_at { get; set; }

        /// <summary>
        /// 기준화폐
        /// </summary>
        [JsonIgnore]
        public string unit_currency => Symbol.GetUnitCurrency(market);

        /// <summary>
        /// 화폐코드
        /// </summary>
        [JsonIgnore]
        public string code => Symbol.GetCode(market);

        /// <summary>
        /// 취소여부
        /// </summary>
        [JsonIgnore]
        public bool IsDeleted => (Convert.ToDouble(paid_fee) == 0 && Convert.ToInt32(trades_count) == 0) ? true : false;

        /// <summary>
        /// 실현손익
        /// </summary>
        public double PnL { get; set; }

        /// <summary>
        /// 손익률(%)
        /// </summary>
        public double PnLRat { get; set; }

    }
}