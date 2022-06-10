using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class MiningBot : AuditableEntity<string>
    {
        //[StringLength(36)]
        public string UserId { get; set; }

        //[StringLength(36)]
        public string Ticket { get; set; }

        //[StringLength(256)]
        public string MachineName { get; set; }

        //[StringLength(20)]
        public string Version { get; set; }

        //[StringLength(4096)]
        public string CurrentDirectory { get; set; }

        public DateTime? Touched { get; set; }

        public IDomainUser User { get; set; }

        public MiningBotTicket MiningBotTicket { get; set; }
    }
}
