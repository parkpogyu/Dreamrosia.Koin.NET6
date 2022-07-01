using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "주문정보")]
    public class PaperOrderDto : OrderDto
    {

        public string Remark { get; set; }
        public float VolumeRate { get; set; }
    }
}