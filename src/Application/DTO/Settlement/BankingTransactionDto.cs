using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "입/출금정보")]
    public class BankingTransactionDto
    {
        public int Id { get; set; }
        public DateTime done_at { get; set; }
        public string Classification { get; set; }
        public string Contents { get; set; }
        public double Deposit { get; set; }
        public double Withdraw { get; set; }
        public double Balance { get; set; }
        public string Counterparty { get; set; }
        public string Memo { get; set; }
        public string UserCode { get; set; }

        public UserProfileDto AccountHolder { get; set; }

        [JsonIgnore]
        public TransferType TransferType => Withdraw > 0 ? TransferType.withdraw : TransferType.deposit;
    }
}