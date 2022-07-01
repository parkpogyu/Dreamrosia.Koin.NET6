using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "자산추적")]
    public class AssetTrackingDto
    {
        public bool IsOrder { get; set; }

        #region Order
        public string market { get; set; }
        public OrderSide side { get; set; }
        public OrderType ord_type { get; set; }
        public decimal? volume { get; set; }
        public double? price { get; set; }
        public decimal executed_volume { get; set; }
        public double paid_fee { get; set; }

        public double avg_price { get; set; }
        public double exec_amount { get; set; }
        public double PnL { get; set; }
        public float PnLRat { get; set; }

        public void SetPnL(double avg_buy_price)
        {
            PnL = (avg_price - avg_buy_price) * (double)executed_volume;

            PnLRat = (float)Ratio.ToSignedPercentage(avg_price, avg_buy_price);
        }
        #endregion

        #region Transfer
        public TransferType type { get; set; }
        public string code { get; set; }
        public TransferState TransferState { get; set; }
        public decimal amount { get; set; }
        public double fee { get; set; }
        #endregion

        #region Common
        public string uuid { get; set; }
        public string txid { get; set; }
        public DateTime done_at { get; set; }
        #endregion
    }
}