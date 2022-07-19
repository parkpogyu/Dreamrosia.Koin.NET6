using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class DelistingSymbol : AuditableEntity<string>
    {
        public string korean_name { get; set; }
        public DateTime NotifiedAt { get; set; }
        public DateTime CloseAt { get; set; }
    }
}
