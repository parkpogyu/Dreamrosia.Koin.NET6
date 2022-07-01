using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Domain.Entities
{
    [Display(Name = "매매신호정보")]
    public class SeasonSignal : AuditableEntity<int>
    {
        public string UserId { get; set; }

        public string market { get; set; }

        public SeasonSignals DailySignal { get; set; }
        public SeasonSignals WeeklySignal { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IDomainUser User { get; set; }
        public Symbol Symbol { get; set; }
    }
}
