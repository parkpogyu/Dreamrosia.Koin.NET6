using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "주문정보")]
    public class OrderDetails : Order
    {
        public IEnumerable<OrderTrade> Trades { get; set; }
    }
}
