using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "화폐정보")]
    public class Currency
    {
        // 타입          필드            설명
        //-------------------------------------------------------------------------------------
        // String        code           화폐를 의미하는 영문 대문자 코드  
        // NumberString  withdraw_fee   해당 화폐의 출금 수수료   
        // Boolean       is_coin        화폐의 코인 여부   
        // String        wallet_state   해당 화폐의 지갑 상태 
        // Array[String] wallet_support 해당 화폐가 지원하는 입출금 정보  
        //-------------------------------------------------------------------------------------

        public string code { get; set; }
        public decimal withdraw_fee { get; set; }
        public bool is_coin { get; set; }
        public string wallet_state { get; set; }
        public List<TransferType> wallet_support { get; set; }

        public static string KRW => "KRW";
        public static string BTC => "BTC";
        public static string USDT => "USDT";
    }
}
