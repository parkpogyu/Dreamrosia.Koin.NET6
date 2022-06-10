using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class UPbitKey : AuditableEntity<string>
    {
        // Id <-> UserId

        //[StringLength(100)]
        public string access_key { get; set; }

        //[StringLength(100)]
        public string secret_key { get; set; }

        public bool IsAuthenticated { get; set; }

        public DateTime? expire_at { get; set; }

        public bool IsOccurredFatalError { get; set; }

        public string FatalError { get; set; }

        public IDomainUser User { get; set; }
    }
}
