using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "보유코인")]
    public class PaperPositionDto : PositionDto
    {
        public DateTime created_at { get; set; }
        public bool IsCleared { get; set; }
    }
}