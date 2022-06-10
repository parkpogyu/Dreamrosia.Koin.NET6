using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
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

        public double amount { get; set; }

        public double fee { get; set; }

        public TransferTransaction transaction_type { get; set; }
    }
}