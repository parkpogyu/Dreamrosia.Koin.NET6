using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "출금가능정보")]
    public class WithdrawAvailable
    {
        [JsonProperty("member_level")]
        [Display(Name = "보안등급정보")]
        public MemberLevel MemberLevel { get; set; }

        [Display(Name = "화폐")]
        public Currency Currency { get; set; }

        [JsonProperty("Account")]
        [Display(Name = "계좌")]
        public Position Position { get; set; }

        [JsonProperty("withdraw_limit")]
        [Display(Name = "출금제약정보")]
        public WithdrawLimit WithdrawLimit { get; set; }
    }
}
