using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "심볼신호정보")]
    public class SeasonSignalDto
    {
        public string UserId { get; set; }

        public string market { get; set; }

        public SeasonSignals DailySignal { get; set; } = SeasonSignals.Indeterminate;

        public SeasonSignals WeeklySignal { get; set; } = SeasonSignals.Indeterminate;

        public DateTime UpdatedAt { get; set; }
    }
}