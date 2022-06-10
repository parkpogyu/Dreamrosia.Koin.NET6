using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Domain.Enums;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Point : AuditableEntity<int>
    {
        //[Required]
        public int TransactionId { get; set; }

        //[Required]
        //[StringLength(36)]
        public string UserId { get; set; }

        //[Required]
        public DateTime done_at { get; set; }

        //[Required]
        public MembershipLevel Membership { get; set; }

        public PointType Type { get; set; }

        public long Amount { get; set; }

        //[Required]
        public long Balance { get; set; }

        public BankingTransaction Transaction { get; set; }

        public IDomainUser User { get; set; }
    }
}