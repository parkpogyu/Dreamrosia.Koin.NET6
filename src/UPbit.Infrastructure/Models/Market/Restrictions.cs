using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "거래제약사항")]
    public class Restrictions
    {
        [JsonProperty("currency")]
        [Display(Name = "화폐코드")]
        public string code { get; set; }

        [Display(Name = "주문단위")]
        public string price_unit { get; set; }

        [Display(Name = "최소 거래금액")]
        public double min_total { get; set; }
    }
}
