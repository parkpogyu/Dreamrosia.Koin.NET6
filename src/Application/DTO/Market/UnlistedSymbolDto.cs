using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "비상장심볼정보")]
    public class UnlistedSymbolDto
    {
        public string code { get; set; }
        public string english_name { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
    }
}