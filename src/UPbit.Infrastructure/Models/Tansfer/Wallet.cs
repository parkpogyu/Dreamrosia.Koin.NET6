using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "지갑정보")]
    public class Wallet
    {
        [JsonProperty("currency")]
        [Display(Name = "화폐코드")]
        public string code { get; set; }

        [Display(Name = "입금주소")]
        public string deposit_address { get; set; }

        [Display(Name = "보조주소")]
        public string secondary_address { get; set; }
    }
}
