using Dreamrosia.Koin.Shared.Constants.Application;
using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class BackTestingRequestDto : TradingTermsDto
    {
        public long SeedMoney { get; set; } = StaticValue.TradingTerms.MaximumAsset4Basic;
        public DateTime? HeadDate { get; set; } = new DateTime(2017, 09, 25);
        public DateTime? RearDate { get; set; } = DateTime.Now.Date;
        public bool IncludeOrders { get; set; }
        public bool IncludePositons { get; set; }

        #region Wrapper for MudNumericField
        public float _SeedMoney
        {
            get => SeedMoney;
            set => SeedMoney = (long)value;
        }

        public float _Amount
        {
            get => Amount;
            set => Amount = (int)value;
        }

        public float _Minimum
        {
            get => Minimum;
            set => Minimum = (int)value;
        }
        public float _Maximum
        {
            get => Maximum;
            set => Maximum = (int)value;
        }
        #endregion
    }
}