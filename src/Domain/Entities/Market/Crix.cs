using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Crix : AuditableEntity<string>
    {
        /// CRIX 마켓코드
        /// Id <-> crix_code

        /// <summary>
        /// 한글명
        /// </summary>
        //[Required]
        //[StringLength(50)]
        public string koreanName { get; set; }

        /// <summary>
        /// 영문명
        /// </summary>
        //[Required]
        //[StringLength(100)]
        public string englishName { get; set; }

        /// <summary>
        /// 화폐코드
        /// </summary>
        //[Required]
        //[StringLength(10)]
        public string code { get; set; }

        /// <summary>
        /// 기준화폐
        /// </summary>
        //[Required]
        //[StringLength(10)]
        public string unit_currency { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        //[Required]
        public double price { get; set; }

        /// <summary>
        /// 시가총액 
        /// </summary>
        public double? marketCap { get; set; }

        /// <summary>
        /// 거래대금(24H)
        /// </summary>
        public double accTradePrice24h { get; set; }

        /// <summary>
        /// 등락률(%) 1H
        /// </summary>
        public double signedChangeRate1h { get; set; }

        /// <summary>
        /// 등락률(%) 24H
        /// </summary>
        public double signedChangeRate24h { get; set; }

        /// <summary>
        /// 발행량
        /// </summary>
        public double availableVolume { get; set; }

        /// <summary>
        /// 정보제공자
        /// </summary>
        public string provider { get; set; }

        /// <summary>
        /// 갱신일시
        /// </summary>
        //[Required]
        public DateTime lastUpdated { get; set; } // 2021-05-30T12:18:02+09:00

        /// <summary>
        /// 갱신일시
        /// </summary>
        public long timestamp { get; set; }
    }
}
