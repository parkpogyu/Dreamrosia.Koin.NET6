using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Domain.Entities
{
    [Display(Name = "매매신호정보")]
    public class SeasonSignal : AuditableEntity<int>
    {
        /// <summary>
        /// 사용자 아이디
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 마켓코드
        /// </summary>
        //[Required]
        //[StringLength(20)]
        public string market { get; set; }

        /// <summary>
        /// 일간신호
        /// </summary>
        //[Required]
        public SeasonSignals DailySignal { get; set; }

        /// <summary>
        /// 주간신호
        /// </summary>
        //[Required]
        public SeasonSignals WeeklySignal { get; set; }

        /// <summary>
        /// 갱신일시
        /// </summary>
        //[Required]
        public DateTime UpdatedAt { get; set; }

        public IDomainUser User { get; set; }

        public Symbol Symbol { get; set; }
    }
}
