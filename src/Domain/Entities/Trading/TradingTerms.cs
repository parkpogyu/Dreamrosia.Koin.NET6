using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class TradingTerms : AuditableEntity<string>
    {
        // Id <-> UserId

        #region Ask Terms
        public bool UseTakeProfit { get; set; }
        public float TakeProfit { get; set; }
        public bool UseStopLoss { get; set; }
        public float StopLoss { get; set; }
        public bool UseTrailingStop { get; set; }
        public float TrailingStopStart { get; set; }
        public float TrailingStop { get; set; }
        public bool LiquidatePositions { get; set; }
        #endregion

        #region Bid Terms
        public BidAmountOption AmountOption { get; set; }
        public float AmountRate { get; set; }
        public int Amount { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public bool Pyramiding { get; set; }
        public bool ApplyMarketPrice { get; set; }
        #endregion

        #region General Terms
        public bool AutoTrading { get; set; }
        public TimeFrames TimeFrame { get; set; }
        public bool Rebalancing { get; set; }
        public OrderBy RebalancingOrder { get; set; }
        #endregion

        public IDomainUser User { get; set; }
    }
}
