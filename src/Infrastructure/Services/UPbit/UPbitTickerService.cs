using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Hubs;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Hubs;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UPbitTickerService : IUPbitTickerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<SynchronizeHub, ISynchronizeClient> _hubContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitTickerService> _logger;

        private readonly Stopwatch _stopwatch;

        public Dictionary<string, WsTicker.WsResponse> Tickers { get; private set; } = new Dictionary<string, WsTicker.WsResponse>();

        private readonly WsTicker WsTicker = new WsTicker();

        private IEnumerable<string> Codes = new List<string>();

        public UPbitTickerService(IServiceProvider serviceProvider,
                                  IHubContext<SynchronizeHub, ISynchronizeClient> hubContext,
                                  IMapper mapper,
                                  ILogger<UPbitTickerService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _mapper = mapper;
            _logger = logger;

            WsTicker.OnMessageReceived += WsTicker_OnMessageReceived;

            _stopwatch = new Stopwatch();
        }

        public async Task<IResult> InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var symbolService = scope.ServiceProvider.GetRequiredService<ISymbolService>();
            var candleService = scope.ServiceProvider.GetRequiredService<ICandleService>();

            var codes = await symbolService.GetSymbolCodesAsync();
            var candles = await candleService.GetLastCandlesAsync(codes.Data);

            _stopwatch.Start();

            if (candles.Succeeded && codes.Succeeded)
            {
                var items = (from lt in codes.Data
                             from rt in candles.Data.Where(f => f.market.Equals(lt)).DefaultIfEmpty()
                             select ((Func<SymbolDto>)(() =>
                             {
                                 return new SymbolDto()
                                 {
                                     market = lt,
                                     trade_price = rt is null ? 0 : rt.Candle.trade_price
                                 };

                             }))()).ToArray();

                foreach (var item in items)
                {
                    Tickers[item.market] = new WsTicker.WsResponse()
                    {
                        market = item.market,
                        trade_price = item.trade_price
                    };
                }

                WsTicker.Send(Tickers.Keys);

                return await Result.SuccessAsync();
            }
            else
            {
                List<string> messages = new List<string>();

                messages.AddRange(candles.Messages);
                messages.AddRange(codes.Messages);

                return await Result.FailAsync(messages);
            }
        }

        private void WsTicker_OnMessageReceived(WsTicker.WsResponse message)
        {
            if (message is null) { return; }

            if (Tickers.ContainsKey(message.code))
            {
                if (Tickers[message.code].trade_price != message.trade_price)
                {
                    Tickers[message.code] = message;
                }
            }
            else
            {
                Tickers.Add(message.code, message);
            }

            Task.Run(async () =>
            {
                await _hubContext.Clients.All.ReceiveTicker(_mapper.Map<TickerDto>(message));

                await SaveTickerToCandle();
            });
        }

        public void RequestTickers(IEnumerable<string> codes)
        {
            var items = from lt in codes
                        from rt in Codes.Where(f => f.Equals(lt))
                        select lt;

            if (codes.Count() == items.Count()) { return; }

            Codes = codes;

            WsTicker.Send(codes);
        }

        public async Task<IResult<IEnumerable<TickerDto>>> GetTradePricesAsync()
        {
            try
            {
                var mapped = _mapper.Map<IEnumerable<TickerDto>>(Tickers.Values);

                return await Result<IEnumerable<TickerDto>>.SuccessAsync(mapped);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<TickerDto>>.FailAsync(ex.Message);
            }
        }

        public async Task<IResult<IEnumerable<DelistingSymbolDto>>> GetDelistingSymbolsAsync()
        {
            try
            {
                var items = Tickers.Values.Where(f => f.delisting_date is not null).ToArray();

                var mapped = _mapper.Map<IEnumerable<DelistingSymbolDto>>(items);

                return await Result<IEnumerable<DelistingSymbolDto>>.SuccessAsync(mapped);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<DelistingSymbolDto>>.FailAsync(ex.Message);
            }
        }


        private async Task SaveTickerToCandle()
        {
            if (!IsSaveTickerToCandle()) { return; }

            using var scope = _serviceProvider.CreateScope();

            var candleService = scope.ServiceProvider.GetRequiredService<ICandleService>();

            await candleService.SaveCandlesAsync(_mapper.Map<IEnumerable<CandleDto>>(Tickers.Values));

            bool IsSaveTickerToCandle()
            {
                DateTime now = DateTime.Now;

                int elapsed = 60000;

                // 09:00 ~ 09:10분 사이는 1초 간격 확인 
                if (9 <= now.Hour && now.Hour < 10 && now.Minute < 10)
                {
                    elapsed = 1000;
                }

                if (elapsed < _stopwatch.ElapsedMilliseconds)
                {
                    _stopwatch.Restart();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
