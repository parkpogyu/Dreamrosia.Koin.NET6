﻿using AutoMapper;
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
using System.Linq;
using System.Threading.Tasks;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UPbitTickerService : IUPbitTickerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<SynchronizeHub, ISynchronizeClient> _hubContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitTickerService> _logger;

        public Dictionary<string, UPbitModels.Ticker> Tickers { get; private set; } = new Dictionary<string, UPbitModels.Ticker>();

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
        }

        public async Task<IResult<int>> InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var symbolService = scope.ServiceProvider.GetRequiredService<ISymbolService>();
            var candleService = scope.ServiceProvider.GetRequiredService<ICandleService>();

            var codes = await symbolService.GetSymbolCodesAsync();
            var candles = await candleService.GetLastCandlesAsync(codes.Data);

            if (candles.Succeeded && codes.Succeeded)
            {
                var items = (from lt in codes.Data
                             from rt in candles.Data.Where(f => f.market.Equals(lt)).DefaultIfEmpty()
                             select ((Func<SymbolDto>)(() =>
                             {
                                 return new SymbolDto()
                                 {
                                     market = lt,
                                     trade_price = rt?.Candle?.trade_price
                                 };

                             }))()).ToArray();

                foreach (var item in items)
                {
                    Tickers[item.market] = new UPbitModels.Ticker()
                    {
                        market = item.market,
                        trade_price = Convert.ToDouble(item.trade_price)
                    };
                }

                WsTicker.Send(Tickers.Keys);

                return await Result<int>.SuccessAsync(items.Count());
            }
            else
            {
                List<string> messages = new List<string>();

                messages.AddRange(candles.Messages);
                messages.AddRange(codes.Messages);

                return await Result<int>.FailAsync(messages);
            }
        }

        private void WsTicker_OnMessageReceived(WsTicker.WsResponse message)
        {
            if (message is null) { return; }

            if (Tickers.ContainsKey(message.code))
            {
                if (Tickers[message.code].trade_price == message.trade_price) { return; }

                Tickers[message.code] = message;
            }
            else
            {
                Tickers.Add(message.code, message);
            }

            Task.Run(async () =>
            {
                await _hubContext.Clients.All.ReceiveTicker(_mapper.Map<TickerDto>(message));
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
    }
}
