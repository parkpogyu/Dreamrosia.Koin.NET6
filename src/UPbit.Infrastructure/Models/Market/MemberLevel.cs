using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "보안등급정보")]
    public class MemberLevel
    {
        [Display(Name = "보안등급")]
        public int security_level { get; set; }

        [Display(Name = "수수료등급")]
        public int fee_level { get; set; }

        [Display(Name = "이메일 인증여부")]
        public bool email_verified { get; set; }

        [Display(Name = "실명 인증여부")]
        public bool identity_auth_verified { get; set; }

        [Display(Name = "계좌 인증여부")]
        public bool bank_account_verified { get; set; }

        [Display(Name = "카카오페이 인증여부")]
        public bool kakao_pay_auth_verified { get; set; }

        [Display(Name = "계정보호상태")]
        public bool locked { get; set; }

        [Display(Name = "출금보호상태")]
        public bool wallet_locked { get; set; }
    }
}
