using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.Models
{
    [Display(Name = "터미널 정보")]
    public static class Terminal
    {
        public static string Id { get; set; }

        public static string Ticket { get; set; }

        public static string MachineName { get; set; }

        public static string Version { get; set; }

        public static string CurrentDirectory { get; set; }
    }
}