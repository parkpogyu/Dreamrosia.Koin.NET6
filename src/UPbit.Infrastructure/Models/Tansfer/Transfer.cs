using Dreamrosia.Koin.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "입/출금정보")]
    public class Transfer
    {
        // 타입          필드             설명
        //-------------------------------------------------------------------------------------
        // String       type             입출금 종류 
        // String       uuid             입출금의 고유 아이디  
        // String       currency         화폐를 의미하는 영문 대문자 코드 
        // String       txid             입출금의 트랜잭션 아이디    
        // String       state            입출금 상태   
        // DateString   created_at       입출금 생성 시간 
        // DateString   done_at          입출금 완료 시간    
        // NumberString amount           입출금 금액/수량 
        // NumberString fee              입출금 수수료 
        // String       transaction_type 입출금 유형
        //                               default : 일반입출금
        //                               internal : 바로입출금 
        //-------------------------------------------------------------------------------------

        public TransferType type { get; set; }
        public string uuid { get; set; }
        [JsonProperty("currency")]
        public string code { get; set; }
        public string txid { get; set; }
        public TransferState state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? done_at { get; set; }
        public decimal amount { get; set; }
        public decimal fee { get; set; }
        public TransferTransaction transaction_type { get; set; }
    }
}
