using Dreamrosia.Koin.Shared.Constants.Coin;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "입/출금정보")]
    public class TransferDto
    {
        public string UserId { get; set; }

        public TransferType type { get; set; }
        public string uuid { get; set; }
        public string code { get; set; }
        public string txid { get; set; }
        public TransferState state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? done_at { get; set; }
        public decimal amount { get; set; }
        public decimal fee { get; set; }
        public TransferTransaction transaction_type { get; set; }

        [JsonIgnore]
        public double? Cash => string.IsNullOrEmpty(code) ? null :
                               code.Equals(Currency.Unit.KRW) ? (double)amount : null;

        [JsonIgnore]
        public double? Volume => string.IsNullOrEmpty(code) ? null :
                                 code.Equals(Currency.Unit.KRW) ? null : (double)amount;
    }
}