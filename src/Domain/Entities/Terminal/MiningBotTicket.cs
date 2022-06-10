using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class MiningBotTicket : AuditableEntity<string>
    {
        public DateTime Touched { get; set; }

        public MiningBot MiningBot { get; set; }
    }
}
