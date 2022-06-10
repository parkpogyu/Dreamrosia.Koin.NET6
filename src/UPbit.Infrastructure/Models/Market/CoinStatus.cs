using Dreamrosia.Koin.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "코인상태")]
    public class CoinStatus
    {
        [JsonProperty("currency")]
        [Display(Name = "화폐코드", GroupName = "코인상태")]
        public string code { get; set; }

        [Display(Name = "지갑상태", GroupName = "코인상태")]
        public WalletState wallet_state { get; set; }

        [Display(Name = "블럭상태", GroupName = "코인상태")]
        public BlockState BlockState
        {
            get
            {
                BlockState state;

                return Enum.TryParse(block_state, out state) ? state : BlockState.None;
            }
        }

        [StringLength(10)]
        [Display(Name = "블럭상태", GroupName = "코인상태")]
        public string block_state { get; set; }

        [Display(Name = "블록높이", GroupName = "코인상태")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? block_height { get; set; }

        [Display(Name = "블록갱신시간", GroupName = "코인상태")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? block_updated_at { get; set; }
    }
}
