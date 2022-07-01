using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "보안등급정보")]
    public class MemberLevel
    {
        // 타입    필드                      설명
        //-------------------------------------------------------------------------------------
        // Integer security_level           사용자의 보안등급   
        // Integer fee_level                사용자의 수수료등급 
        // Boolean email_verified           사용자의 이메일 인증 여부 
        // Boolean identity_auth_verified   사용자의 실명 인증 여부 
        // Boolean bank_account_verified    사용자의 계좌 인증 여부
        // Boolean kakao_pay_auth_verified  사용자의 카카오페이 인증 여부 
        // Boolean locked                   사용자의 계정 보호 상태 
        // Boolean wallet_locked            사용자의 출금 보호 상태 
        //-------------------------------------------------------------------------------------

        public int security_level { get; set; }
        public int fee_level { get; set; }
        public bool email_verified { get; set; }
        public bool identity_auth_verified { get; set; }
        public bool bank_account_verified { get; set; }
        public bool kakao_pay_auth_verified { get; set; }
        public bool locked { get; set; }
        public bool wallet_locked { get; set; }
    }
}
