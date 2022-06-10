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

        public double volume { get; set; }

        public double price { get; set; }

        public double executed_volume { get; set; }

        public double paid_fee { get; set; }

        private double _avg_price { get; set; }
        public double avg_price
        {
            get
            {
                return _avg_price == 0 ? price : _avg_price;
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
                return _exec_amount == 0 ? price * executed_volume : _exec_amount;
            }

            set
            {
                _exec_amount = value;
            }
        }

        public double PnL { get; set; }

        public double PnLRat { get; set; }

        public void SetPnL(double avg_buy_price)
        {
            PnL = (avg_price - avg_buy_price) * executed_volume;

            PnLRat = Ratio.ToSignedPercentage(avg_price, avg_buy_price);
        }
        #endregion

        #region Transfer
        public TransferType type { get; set; }

        public string code { get; set; }

        public TransferState TransferState { get; set; }

        public string txid { get; set; }

        public double amount { get; set; }

        public double fee { get; set; }
        #endregion

        #region Common
        public string uuid { get; set; }

        public DateTime done_at { get; set; }
        #endregion
    }
}