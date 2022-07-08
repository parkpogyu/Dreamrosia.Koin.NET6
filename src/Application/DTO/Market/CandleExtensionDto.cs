using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "주가정보")]
    public class CandleExtensionDto : CandleDto
    {
        public double? signal { get; set; }
        public double? index { get; set; }
    }
}