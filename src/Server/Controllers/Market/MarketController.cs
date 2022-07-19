using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : BaseApiController<MarketController>
    {
        private readonly ICandleService _candleService;
        private readonly IMarketIndexService _marketIndexService;
        private readonly ISymbolService _symbolService;
        private readonly IDelistingSymbolService _excludedSymbolService;

        public MarketController(ICandleService candleService,
                                IMarketIndexService marketIndexService,
                                ISymbolService symbolService,
                                IDelistingSymbolService excludedSymbolService)
        {
            _candleService = candleService;
            _marketIndexService = marketIndexService;
            _symbolService = symbolService;
            _excludedSymbolService = excludedSymbolService;
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
        public async Task<IActionResult> GetCandles(string market, DateTime head, DateTime rear)
        {
            var response = await _candleService.GetCandlesAsync(market,
                                                                head.Date,
                                                                rear.Date);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Candles.View)]
        [HttpGet("indices")]
        public async Task<IActionResult> GetMarketIndices(DateTime head, DateTime rear)
        {
            var response = await _marketIndexService.GetMarketIndicesAsync(head.Date, rear.Date);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Symbols.View)]
        [HttpGet("symbols")]
        public async Task<IActionResult> GetSymbols()
        {
            var response = await _symbolService.GetSymbolsAsync();

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Symbols.View)]
        [HttpGet("delistingsymbols")]
        public async Task<IActionResult> GetDelistingSymbols()
        {
            var response = await _excludedSymbolService.GetDelistingSymbolsAsync();

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Symbols.Edit)]
        [HttpPost("delistingsymbols/regist")]
        public async Task<IActionResult> RegistDelistingSymbolAsync(DelistingSymbolDto model)
        {
            var response = await _excludedSymbolService.RegistDelistingSymbolAsync(model);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Symbols.Edit)]
        [HttpPost("delistingsymbols/delete")]
        public async Task<IActionResult> DeleteDelistingSymbolsAsync(IEnumerable<DelistingSymbolDto> models)
        {
            var response = await _excludedSymbolService.DeleteDelistingSymbolsAsync(models);

            return Ok(response);
        }
    }
}