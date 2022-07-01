using Dreamrosia.Koin.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "포인트 정보")]
    public class PointDto
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public DateTime done_at { get; set; }
        public MembershipLevel Membership { get; set; }
        public PointType Type { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }

        [JsonIgnore]
        public DateTime? transaction_at => Transaction is null ? null : Transaction.done_at;
        [JsonIgnore]
        public long Charging => (Type == PointType.Charging || Type == PointType.Coupon) ? Amount : 0;
        [JsonIgnore]
        public long Reedeem => Type == PointType.Redeem ? Amount : 0;
        [JsonIgnore]
        public long Refund => Type == PointType.Refund ? Amount : 0;

        public BankingTransactionDto Transaction { get; set; }
    }
}