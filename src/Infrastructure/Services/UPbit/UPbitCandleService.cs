using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UPbitCandleService : IUPbitCandleService
    {
        private readonly BlazorHeroContext _context;
        private readonly ICandleService _candleService;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitCandleService> _logger;

        public UPbitCandleService(BlazorHeroContext context,
                                  ICandleService candleService,
                                  IMapper mapper,
                                  ILogger<UPbitCandleService> logger)
        {
            _context = context;
            _candleService = candleService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult> GetCandlesAsync()
        {
            var markets = _context.Symbols.Select(f => f.Id);

            var candidates = await _candleService.GetLastCandlesAsync(markets);

            if (!candidates.Succeeded)
            {
                _logger.LogWarning($"GetLastCandlesAsync: {candidates.FullMessage}");

                return await Result.FailAsync(candidates.Messages);
            }

            QtCandle QtCandle = new QtCandle();

            QtCandle.QtParameter parameter = new QtCandle.QtParameter();

            List<UPbitModels.Candle> candles = new List<UPbitModels.Candle>();

            DateTime now = DateTime.UtcNow.Date;

            foreach (var candidate in candidates.Data)
            {
                var last = candidate.Candle;

                DateTime? utc = candidate.Candle?.candle_date_time_utc;

                parameter.TimeFrame = TimeFrames.Day;
                parameter.count = last is null ?
                                  ClientConstants.MaxCount :
                                  (int)now.Subtract(last.candle_date_time_utc).TotalDays + 1;
                parameter.market = candidate.market;
                parameter.to = null;

                while (true)
                {
                    var result = await QtCandle.GetCandlesAsync(parameter);

                    if (!result.Succeeded)
                    {
                        _logger.LogWarning($"GetCandlesAsync {result.FullMessage}");

                        return await Result.FailAsync(result.Messages);
                    }

                    if (!result.Data.Any()) { break; }

                    candles.AddRange(result.Data);

                    if (result.Data.Any(f => f.candle_date_time_utc == utc)) { break; }

                    parameter.to = result.Data.Min(f => f.candle_date_time_utc).AddDays(-1);
                }
            }

            var items = _mapper.Map<IEnumerable<CandleDto>>(candles);

            var saved = await _candleService.SaveCandlesAsync(items);

            if (saved.Succeeded)
            {
                return await Result.SuccessAsync();
            }
            else
            {
                _logger.LogWarning($"SaveCandlesAsync: {saved.FullMessage}");

                return await Result.FailAsync(saved.Messages);
            }
        }

        public async Task<IResult> GetCandlesAsync(TimeFrames frame, IEnumerable<string> markets)
        {
            int count = frame == TimeFrames.Week ? 7 :
                        frame == TimeFrames.Month ? 31 :
                        frame == TimeFrames.Year ? 365 : 1;

            IEnumerable<UPbitModels.Candle> candles = null;

            foreach (var market in markets)
            {
                candles = await GetCandlesAsync(market, count);

                if (candles is null) { continue; }

                await _candleService.SaveCandlesAsync(_mapper.Map<IEnumerable<CandleDto>>(candles));
            }

            return await Result.SuccessAsync();
        }

        private async Task<IEnumerable<UPbitModels.Candle>> GetCandlesAsync(string market, int count)
        {
            QtCandle QtCandle = new QtCandle();

            QtCandle.QtParameter parameter = new QtCandle.QtParameter();

            List<UPbitModels.Candle> candles = new List<UPbitModels.Candle>();

            parameter.TimeFrame = TimeFrames.Day;
            parameter.market = market;
            parameter.count = count > ClientConstants.MaxCount ? ClientConstants.MaxCount : count;

            while (true)
            {
                var result = await QtCandle.GetCandlesAsync(parameter);

                if (!result.Succeeded)
                {
                    _logger.LogWarning($"GetCandlesAsync {result.FullMessage}");

                    return null;
                }

                if (!result.Data.Any()) { break; }

                candles.AddRange(result.Data);

                if (count <= result.Data.Count()) { break; }

                parameter.to = result.Data.Min(f => f.candle_date_time_utc).AddDays(-1);
            }

            return candles.Take(count).ToArray();
        }
    }
}
