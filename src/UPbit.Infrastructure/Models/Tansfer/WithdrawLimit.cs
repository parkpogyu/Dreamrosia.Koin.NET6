using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "출금제약정보")]
    public class WithdrawLimit
    {
        [JsonProperty("currency")]
        [Display(Name = "화폐코드", GroupName = "출금제약정보")]
        public string code { get; set; }

        [Display(Name = "최소금액/수량", GroupName = "출금제약정보")]
        public double? minimum { get; set; }

        [Display(Name = "1회 출금한도", GroupName = "출금제약정보")]
        public double? onetime { get; set; }

        [Display(Name = "1일 출금한도", GroupName = "출금제약정보")]
        public double? daily { get; set; }

        [Display(Name = "1일 잔여한도", GroupName = "출금제약정보")]
        public double? remaining_daily { get; set; }

        [Display(Name = "1일 잔여한도(통합)", GroupName = "출금제약정보")]
        public double? remaining_daily_krw { get; set; }

        [Display(Name = "출금금액/수량", GroupName = "출금제약정보")]
        public int? @fixed { get; set; }

        [Display(Name = "출금지원여부", GroupName = "출금제약정보")]
        public bool can_withdraw { get; set; }
    }
}
