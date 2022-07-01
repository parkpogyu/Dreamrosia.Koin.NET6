using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "인증키정보")]
    public class UPbitKey
    {
        public string access_key { get; set; }
        public string secret_key { get; set; }
        /// <summary>
        /// 인증만료일
        /// </summary>
        public DateTime? expire_at { get; set; }
    }
}
