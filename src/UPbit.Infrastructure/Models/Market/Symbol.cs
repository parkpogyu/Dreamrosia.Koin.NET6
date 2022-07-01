using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "심볼정보")]
    public class Symbol
    {

        // 타입  필드            설명
        //-------------------------------------------------------------------------------------
        //String market         업비트에서 제공중인 시장 정보 
        //String korean_name    거래 대상 암호화폐 한글명 
        //String english_name   거래 대상 암호화폐 영문명
        //String market_warning 유의 종목 여부
        //                      NONE(해당 사항 없음), CAUTION(투자유의)  
        //-------------------------------------------------------------------------------------

        private string _market { get; set; }
        public string market
        {
            get => _market;
            set
            {
                _market = value;

                var splits = _market.Split("-", StringSplitOptions.RemoveEmptyEntries);

                unit_currency = splits[0];
                code = splits[1];
            }
        }
        public string korean_name { get; set; }
        public string english_name { get; set; }
        public MarketAlert market_warning { get; set; } = MarketAlert.None;

        #region Append
        public string unit_currency { get; private set; }

        public string code { get; private set; }
        #endregion
    }
}
