using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class TradingTerms : AuditableEntity<string>
    {
        // Id <-> UserId

        #region Ask Terms
        //[Required]
        public bool UseTakeProfit { get; set; } = true;

        //[Required]
        public float TakeProfit { get; set; } = 500F;

        //[Required]
        public bool UseStopLoss { get; set; } = true;

        //[Required]
        public float StopLoss { get; set; } = -50F;

        //[Required]
        public bool UseTrailingStop { get; set; } = false;

        //[Required]
        public float TrailingStopStart { get; set; } = 1000F;

        //[Required]
        public float TrailingStop { get; set; } = 15F;

        //[Required]
        public bool LiquidatePositions { get; set; } = false;
        #endregion

        #region Bid Terms
        public BidAmountOption AmountOption { get; set; } = BidAmountOption.Auto;

        public float AmountRate { get; set; } = 1F;

        public long Amount { get; set; } = 10000;

        public long Minimum { get; set; } = 10000;

        public long Maximum { get; set; } = 0;

        public bool Pyramiding { get; set; } = false;

        public bool ApplyMarketPrice { get; set; } = true;
        #endregion

        #region General Terms
        //[Required]
        public bool AutoTrading { get; set; } = false;

        //[Required]
        public TimeFrames TimeFrame { get; set; }
        #endregion

        public IDomainUser User { get; set; }
    }
}
