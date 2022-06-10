using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "업비트 인증 테스트")]
    public class UPbitKeyTestDto : UPbitKeyDto
    {
        public bool IsAllowedPositions { get; set; }
        public bool IsAllowedOrder { get; set; }
        public bool IsAllowedOrders { get; set; }
        public bool IsAllowedWithdraws { get; set; }
        public bool IsAllowedDeposits { get; set; }
        public List<string> Messages { get; set; } = new List<string>();

        public bool IsPassed { get; set; }
    }
}