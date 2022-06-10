using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "보유코인")]
    public class PaperPositionDto : PositionDto
    {
        [Display(Name = "보유일자", GroupName = "계좌정보")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime created_at { get; set; }

        public bool IsCleared { get; set; }
    }
}