using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "매매조건 정보")]
    public class TradingTermsDto
    {
        public string Ticket { get; set; }

        public string UserId { get; set; }

        #region Ask Terms
        public bool UseTakeProfit { get; set; } = true;

        public float TakeProfit { get; set; } = 500F;

        public bool UseStopLoss { get; set; } = true;

        public float StopLoss { get; set; } = -50F;

        public bool UseTrailingStop { get; set; } = false;

        public float TrailingStopStart { get; set; } = 1000F;

        public float TrailingStop { get; set; } = 15F;

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
        public bool AutoTrading { get; set; } = false;

        public TimeFrames TimeFrame { get; set; } = TimeFrames.Day;

        public long MaximumAsset { get; set; } = 1500000;
        #endregion

        public UPbitKeyDto UPbitKey { get; set; }

        public IEnumerable<SeasonSignalDto> Signals { get; set; }

        public OrderDto LastOrder { get; set; }

        public TransferDto LastDeposit { get; set; }

        public TransferDto LastWithdraw { get; set; }
    }
}