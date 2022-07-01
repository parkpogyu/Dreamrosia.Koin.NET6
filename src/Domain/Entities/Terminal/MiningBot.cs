using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class MiningBot : AuditableEntity<string>
    {
        public string Ticket { get; set; }
        public string MachineName { get; set; }
        public string Version { get; set; }
        public string CurrentDirectory { get; set; }
        public DateTime? Touched { get; set; }

        public MiningBotTicket MiningBotTicket { get; set; }
    }
}
