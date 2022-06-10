using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Domain.Contracts
{
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }

        [StringLength(36)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [StringLength(36)]
        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}