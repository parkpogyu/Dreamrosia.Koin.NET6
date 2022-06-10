using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class BankingTransaction : AuditableEntity<int>
    {
        public DateTime done_at { get; set; }
        public string Classification { get; set; }
        public string Contents { get; set; }
        public double Deposit { get; set; }
        public double Withdraw { get; set; }
        public double Balance { get; set; }
        public string Counterparty { get; set; }
        public string Memo { get; set; }
        public string UserCode { get; set; }
        public bool IsExcluded { get; set; }
        public bool IsApplied { get; set; }

        public Point Point { get; set; }
    }
}