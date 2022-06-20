using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class TradingTerms : AuditableEntity<string>
    {
        // Id <-> UserId

        #region Ask Terms
        //[Required]
        public bool UseTakeProfit { get; set; }

        //[Required]
        public float TakeProfit { get; set; }

        //[Required]
        public bool UseStopLoss { get; set; }

        //[Required]
        public float StopLoss { get; set; }

        //[Required]
        public bool UseTrailingStop { get; set; }

        //[Required]
        public float TrailingStopStart { get; set; }

        //[Required]
        public float TrailingStop { get; set; }

        //[Required]
        public bool LiquidatePositions { get; set; }
        #endregion

        #region Bid Terms
        public BidAmountOption AmountOption { get; set; }

        public float AmountRate { get; set; }

        public long Amount { get; set; }

        public long Minimum { get; set; }

        public long Maximum { get; set; }

        public bool Pyramiding { get; set; }

        public bool ApplyMarketPrice { get; set; }
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
