using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Transfer : AuditableEntity<string>
    {
        /// 거래번호
        /// Id <-> uuid

        //[Required]
        //[StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// 거래종류
        /// </summary>
        //[Required]
        public TransferType type { get; set; }

        /// <summary>
        /// 화폐코드
        /// </summary>
        //[Required]
        //[StringLength(10)]
        public string code { get; set; }

        /// <summary>
        /// 거래 요청번호
        /// </summary>
        //[StringLength(100)]
        public string txid { get; set; }

        /// <summary>
        /// 거래상태
        /// </summary>
        //[Required]
        public TransferState state { get; set; }

        /// <summary>
        /// 요청일시
        /// </summary>
        //[Required]
        public DateTime created_at { get; set; }

        /// <summary>
        /// 완료일시
        /// </summary>
        public DateTime? done_at { get; set; }

        /// <summary>
        /// 금액/수량
        /// </summary>
        //[Required]
        public double amount { get; set; }

        /// <summary>
        /// 수수료
        /// </summary>
        public double fee { get; set; }

        /// <summary>
        /// 거래유형
        /// </summary>
        //[Required]
        public TransferTransaction transaction_type { get; set; }

        public IDomainUser User { get; set; }
    }
}
