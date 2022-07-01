using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "보유코인")]
    public class Position
    {
        // 타입          필드                     설명
        //-----------------------------------------------------------------------
        // String        currency                화폐를 의미하는 영문 대문자 코드 
        // NumberString  balance                 주문가능 금액/수량               
        // NumberString  locked                  주문 중 묶여있는 금액/수량       
        // NumberString  avg_buy_price           매수평균가                       
        // Boolean       avg_buy_price_modified  매수평균가 수정 여부             
        // String        unit_currency           평단가 기준 화폐                 
        //-----------------------------------------------------------------------

        [JsonProperty("currency")]
        public string code { get; set; }
        public decimal balance { get; set; }
        public decimal locked { get; set; }
        public double avg_buy_price { get; set; }
        public bool avg_buy_price_modified { get; set; }
        public string unit_currency { get; set; }
    }
}
