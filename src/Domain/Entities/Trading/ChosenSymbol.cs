using Dreamrosia.Koin.Domain.Contracts;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class ChosenSymbol : AuditableEntity<int>
    {
        //[Required]
        //[StringLength(36)]
        public string UserId { get; set; }

        public string market { get; set; }

        public IDomainUser User { get; set; }

        public Symbol Symbol { get; set; }
    }
}
