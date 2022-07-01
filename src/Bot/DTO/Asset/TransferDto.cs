using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "입/출금정보")]
    public class TransferDto : Transfer
    {
        public string UserId { get; set; }
    }
}