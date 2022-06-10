using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "마이닝 봇")]
    public class MiningBotDto
    {
        public string Id { get; set; }

        public string Ticket { get; set; }

        public string MachineName { get; set; }

        public string Version { get; set; }

        public string CurrentDirectory { get; set; }

        public DateTime? Touched { get; set; }
    }
}