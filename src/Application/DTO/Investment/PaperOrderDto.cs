using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "주문정보")]
    public class PaperOrderDto : OrderDto
    {

        [Display(Name = "비고", GroupName = "주문정보")]
        public string Remark { get; set; }

        [Display(Name = "거래비율(%)", GroupName = "주문정보")]
        [DisplayFormat(DataFormatString = "{0:N8}")]
        public double VolumeRate { get; set; }
    }
}