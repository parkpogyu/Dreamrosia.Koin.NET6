using Dreamrosia.Koin.Domain.Contracts;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Position : AuditableEntity<int>
    {
        //[Required]
        //[StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// 화폐코드
        /// </summary>
        //[Required]
        //[StringLength(10)]
        public string code { get; set; }

        /// <summary>
        /// 금액/수량
        /// </summary>
        //[Required]
        public double balance { get; set; }

        /// <summary>
        /// 묶여있는 금액/수량
        /// </summary>
        public double locked { get; set; }

        /// <summary>
        /// 평균단가
        /// </summary>
        //[Required]
        public double avg_buy_price { get; set; }

        /// <summary>
        /// 평균단가 수정 여부
        /// </summary>
        public bool avg_buy_price_modified { get; set; }

        /// <summary>
        /// 기준화폐
        /// </summary>
        //[Required]
        //[StringLength(10)]
        public string unit_currency { get; set; }

        public IDomainUser User { get; set; }
    }
}
