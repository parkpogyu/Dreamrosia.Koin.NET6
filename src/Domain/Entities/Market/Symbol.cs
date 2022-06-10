﻿using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dreamrosia.Koin.Domain.Entities
{
    [Display(Name = "심볼정보")]
    public class Symbol : AuditableEntity<string>
    {
        /// 마켓코드
        /// Id <-> market

        /// <summary>
        /// 한글명
        /// </summary>
        //[Required]
        //[StringLength(50)]
        public string korean_name { get; set; }

        /// <summary>
        /// 영문명
        /// </summary>
        //[Required]
        //[StringLength(100)]
        public string english_name { get; set; }

        /// <summary>
        /// 투자주의
        /// </summary>
        //[Required]
        public MarketAlert market_warning { get; set; }

        public ICollection<SeasonSignal> Signals { get; set; }

        public ICollection<ChosenSymbol> ChosenSymbols { get; set; }

        public static string GetUnitCurrency(string market)
        {
            if (string.IsNullOrEmpty(market)) { return string.Empty; }

            var splits = market.Split("-", StringSplitOptions.RemoveEmptyEntries);

            return splits.Count() == 2 ? splits[0] : string.Empty;
        }

        public static string GetCode(string market)
        {
            if (string.IsNullOrEmpty(market)) { return string.Empty; }

            var splits = market.Split("-", StringSplitOptions.RemoveEmptyEntries);

            return splits.Count() == 2 ? splits[1] : string.Empty;
        }
    }
}
