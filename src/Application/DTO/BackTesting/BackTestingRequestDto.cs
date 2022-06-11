using Dreamrosia.Koin.Shared.Constants.Application;
using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class BackTestingRequestDto : TradingTermsDto
    {
        public float SeedMoney { get; set; } = DefaultValue.TradingTerms.MaximumAsset4Basic;

        public DateTime? HeadDate { get; set; } = new DateTime(2017, 09, 25);

        public DateTime? RearDate { get; set; } = DateTime.Now.Date;

        public bool IncludeOrders { get; set; }

        public bool IncludePositons { get; set; }
    }
}