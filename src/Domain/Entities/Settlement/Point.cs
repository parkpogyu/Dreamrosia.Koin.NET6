using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Domain.Enums;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Point : AuditableEntity<int>
    {
        public int? TransactionId { get; set; }

        public string UserId { get; set; }
        public DateTime done_at { get; set; }
        public MembershipLevel Membership { get; set; }
        public PointType Type { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }

        public IDomainUser User { get; set; }
        public BankingTransaction Transaction { get; set; }
    }
}