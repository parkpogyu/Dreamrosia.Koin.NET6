﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "업비트 인증")]
    public class UPbitKeyDto
    {
        public string UserId { get; set; }

        public string access_key { get; set; }

        public string secret_key { get; set; }

        public bool IsAuthenticated { get; set; }

        public DateTime? expire_at { get; set; }

        public bool IsOccurredFatalError { get; set; }
    }
}