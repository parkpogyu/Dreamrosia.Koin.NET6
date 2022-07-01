using Dreamrosia.Koin.Domain.Contracts;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Position : AuditableEntity<int>
    {
        public string UserId { get; set; }

        public string code { get; set; }
        public decimal balance { get; set; }
        public decimal locked { get; set; }
        public double avg_buy_price { get; set; }
        public bool avg_buy_price_modified { get; set; }
        public string unit_currency { get; set; }

        public IDomainUser User { get; set; }
    }
}
