using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Transfer : AuditableEntity<string>
    {
        /// 거래번호
        /// Id <-> uuid

        public string UserId { get; set; }

        public TransferType type { get; set; }
        public string code { get; set; }
        public string txid { get; set; }
        public TransferState state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? done_at { get; set; }
        public decimal amount { get; set; }
        public decimal fee { get; set; }
        public TransferTransaction transaction_type { get; set; }

        public IDomainUser User { get; set; }
    }
}
