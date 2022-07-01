using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "출금가능정보")]
    public class WithdrawAvailable
    {
        [JsonProperty("member_level")]
        public MemberLevel MemberLevel { get; set; }
        public Currency Currency { get; set; }
        [JsonProperty("account")]
        public Position Position { get; set; }
        [JsonProperty("withdraw_limit")]
        public WithdrawLimit WithdrawLimit { get; set; }
    }
}
