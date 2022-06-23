using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "마이닝 봇 티켓")]
    public class MiningBotTicketDto
    {
        public string Id { get; set; }

        public MiningBotDto MiningBot { get; set; }

        public UserCardDto User { get; set; }
    }
}