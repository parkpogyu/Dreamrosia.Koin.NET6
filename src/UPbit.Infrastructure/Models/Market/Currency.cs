using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "화폐")]
    public class Currency
    {
        [Display(Name = "화폐코드", GroupName = "화폐")]
        public string code { get; set; }

        [Display(Name = "출금수수료", GroupName = "화폐")]
        public double withdraw_fee { get; set; }

        [Display(Name = "코인여부", GroupName = "화폐")]
        public bool is_coin { get; set; }

        [Display(Name = "지갑상태", GroupName = "화폐")]
        public string wallet_state { get; set; }

        [Display(Name = "입출금종류", GroupName = "화폐")]
        public List<TransferType> wallet_support { get; set; }

        public static string KRW => "KRW";
        public static string BTC => "BTC";
        public static string USDT => "USDT";
    }
}
