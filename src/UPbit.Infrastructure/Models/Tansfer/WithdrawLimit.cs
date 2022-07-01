using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "출금제약정보")]
    public class WithdrawLimit
    {
        // 타입         필드                 설명
        //-------------------------------------------------------------------------------------
        // String       currency            화폐를 의미하는 영문 대문자 코드 
        // NumberString minimum             출금 최소 금액/수량 
        // NumberString onetime             1회 출금 한도 
        // NumberString daily               1일 출금 한도 
        // NumberString remaining_daily     1일 잔여 출금 한도 
        // NumberString remaining_daily_krw 통합 1일 잔여 출금 한도  
        // Integer      fixed               출금 금액/수량 소수점 자리 수   
        // Boolean      can_withdraw        출금 지원 여부    
        //-------------------------------------------------------------------------------------

        [JsonProperty("currency")]
        public string code { get; set; }
        public decimal minimum { get; set; }
        public decimal onetime { get; set; }
        public decimal daily { get; set; }
        public decimal remaining_daily { get; set; }
        public decimal remaining_daily_krw { get; set; }
        [JsonProperty("fixed")]
        public int decimal_point { get; set; }
        public bool can_withdraw { get; set; }
    }
}
