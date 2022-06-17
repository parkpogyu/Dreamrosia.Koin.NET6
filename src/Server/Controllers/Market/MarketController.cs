﻿using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : BaseApiController<MarketController>
    {
        private readonly ICandleService _candleService;
        private readonly ISymbolService _symbolService;

        public MarketController(ICandleService candleService,
                                ISymbolService symbolService)
        {
            _candleService = candleService;
            _symbolService = symbolService;
        }

        /// <summary>
        /// 시세 조회 
        /// </summary>
        /// <param name="market"></param>
        /// <param name="head"></param>
        /// <param name="rear"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Candles.View)]
        [HttpGet("candles")]
        public async Task<IActionResult> GetCandles(string market, DateTime? head, DateTime? rear)
        {
            var response = await _candleService.GetCandlesAsync(market,
                                                                Convert.ToDateTime(head).ToUniversalDate(),
                                                                Convert.ToDateTime(rear).ToUniversalDate());

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Symbols.View)]
        [HttpGet("symbols")]
        public async Task<IActionResult> GetSymbols()
        {
            var response = await _symbolService.GetSymbolsAsync();

            return Ok(response);
        }
    }
}