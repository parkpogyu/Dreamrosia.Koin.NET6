using Dreamrosia.Koin.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "입/출금정보")]
    public class Transfer
    {
        [Display(Name = "거래종류")]
        public TransferType type { get; set; }

        [Display(Name = "고유번호")]
        public string uuid { get; set; }

        [JsonProperty("currency")]
        [Display(Name = "화폐코드")]
        public string code { get; set; }

        [Display(Name = "거래번호")]
        public string txid { get; set; }

        [Display(Name = "거래상태")]
        public TransferState state { get; set; }

        [Display(Name = "요청시간")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime created_at { get; set; }

        [Display(Name = "완료시간")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public DateTime? done_at { get; set; }

        [Display(Name = "금액/수량")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double amount { get; set; }

        [Display(Name = "금액")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? AmountKRW => code.Equals(Coin.KRW) ? amount : null;

        [Display(Name = "수량")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double? AmountCoin => code.Equals(Coin.KRW) ? null : amount;

        [Display(Name = "수수료")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double fee { get; set; }

        [Display(Name = "원화환산가격")]
        public double krw_amount { get; set; }

        [Display(Name = "거래유형")]
        public TransferTransaction transaction_type { get; set; }

    }
}
