using Dreamrosia.Koin.Shared.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "코인상태")]
    public class CoinStatus
    {
        // 타입      필드            설명
        //-------------------------------------------------------------------------------------
        // String    currency           화폐를 의미하는 영문 대문자 코드  
        // String    wallet_state       입출금 상태
        //                              - working : 입출금 가능
        //                              - withdraw_only : 출금만 가능
        //                              - deposit_only : 입금만 가능
        //                              - paused : 입출금 중단
        //                              - unsupported : 입출금 미지원 
        // String     block_state       블록 상태
        //                              - normal : 정상
        //                              - delayed : 지연
        //                              - inactive : 비활성(점검 등) 
        // Integer    block_height      블록 높이   
        // DateString block_updated_at  블록 갱신 시각 
        //-------------------------------------------------------------------------------------

        [JsonProperty("currency")]
        public string code { get; set; }
        public WalletState wallet_state { get; set; }
        public BlockState BlockState
        {
            get
            {
                BlockState state;

                return Enum.TryParse(block_state, out state) ? state : BlockState.None;
            }
        }

        public string block_state { get; set; }
        public int block_height { get; set; }
        public DateTime? block_updated_at { get; set; }
    }
}
