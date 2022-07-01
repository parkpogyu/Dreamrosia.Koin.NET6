using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "거래제약사항")]
    public class Restrictions
    {
        // 타입      필드        설명
        //------------------------------------------------------------------------------------
        // String   currency    화폐를 의미하는 영문 대문자 코드  
        // String   price_unit  주문금액 단위 
        // Number   min_total   최소 매도/매수 금액 
        //------------------------------------------------------------------------------------

        [JsonProperty("currency")]
        public string code { get; set; }
        public string price_unit { get; set; }
        public decimal min_total { get; set; }
    }
}
