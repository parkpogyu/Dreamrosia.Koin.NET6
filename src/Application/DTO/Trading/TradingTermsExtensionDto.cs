using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "매매조건 정보")]
    public class TradingTermsExtensionDto : TradingTermsDto
    {
        public string BotId { get; set; }

        public IEnumerable<SeasonSignalDto> Signals { get; set; }

        public OrderDto LastOrder { get; set; }

        public TransferDto LastDeposit { get; set; }

        public TransferDto LastWithdraw { get; set; }
    }
}