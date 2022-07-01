using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "업비트 인증")]
    public class UPbitKeyDto : UPbitKey
    {
        public string UserId { get; set; }

        public bool IsAuthenticated { get; set; }
        public bool IsOccurredFatalError { get; set; }
    }
}