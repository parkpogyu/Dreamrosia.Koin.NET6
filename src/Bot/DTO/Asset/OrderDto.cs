using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "주문정보")]
    public class OrderDto : Order
    {
        public string UserId { get; set; }
    }
}