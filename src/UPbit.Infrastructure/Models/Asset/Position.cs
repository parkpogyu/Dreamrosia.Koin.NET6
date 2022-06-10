using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "보유코인")]
    public class Position
    {
        [JsonProperty("currency")]
        [Display(Name = "화폐코드", GroupName = "계좌정보")]
        public string code { get; set; }

        [Display(Name = "잔고", GroupName = "계좌정보")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double balance { get; set; }

        [Display(Name = "주문대기", GroupName = "계좌정보")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double locked { get; set; }

        [Display(Name = "평균단가", GroupName = "계좌정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double avg_buy_price { get; set; }

        [Display(Name = "평균단가 수정여부", GroupName = "계좌정보")]
        public bool avg_buy_price_modified { get; set; }

        [Display(Name = "기준화폐", GroupName = "계좌정보")]
        public string unit_currency { get; set; }
    }
}
