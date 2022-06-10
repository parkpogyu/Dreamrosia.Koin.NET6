using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "인증")]
    public class UPbitKey
    {
        [Display(Name = "Access Key")]
        public string access_key { get; set; }

        [Display(Name = "Secret Key")]
        public string secret_key { get; set; }

        [Display(Name = "인증 만료일")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? expire_at { get; set; }

        public static int ValidKeyLength => 40;
    }
}
