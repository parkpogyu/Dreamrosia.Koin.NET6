using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "체결정보")]
    public class OrderTrade
    {
        [Display(Name = "마켓코드")]
        public string market { get; set; }

        [Display(Name = "체결번호")]
        public string uuid { get; set; }

        [Display(Name = "체결가격")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double price { get; set; }

        [Display(Name = "체결수량")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double volume { get; set; }

        [Display(Name = "체결총액")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double funds { get; set; }

        [Display(Name = "주문종류")]
        public OrderSide Side { get; set; }

        [Display(Name = "체결시간")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? created_at { get; set; }

        #region Appended
        [Display(Name = "기준화폐")]
        public string unit_currency => Symbol.GetUnitCurrency(market);

        [Display(Name = "화폐코드")]
        public string code => Symbol.GetCode(market);
        #endregion
    }
}
