using Dreamrosia.Koin.Bot.Constants;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Interfaces;
using Dreamrosia.Koin.Bot.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Services
{
    public class SynchronizeService : ISynchronizeService
    {
        private readonly ILogger<SynchronizeService> _logger;

        private readonly HubConnection connection;

        public Dictionary<string, TickerDto> Tickers => Depot.Tickers;

        public SynchronizeService(ILogger<SynchronizeService> logger)
        {
            _logger = logger;

            connection = new HubConnectionBuilder()
                            .WithUrl(ServerConstants.HubUrl)
                            .Build();

            connection.On<TickerDto>(HubMethod.ReceiveTicker, (ticker) =>
            {
                ReceiveTicker(ticker);
            });
        }

        public async Task ConnectHubAsync()
        {
            if (connection.State != HubConnectionState.Disconnected) { return; }

            try
            {
                _logger.LogInformation($"==> SynchronizeHub connecting to {ServerConstants.HubUrl}");

                await Task.Delay(1000);
                await connection.StartAsync();

                if (connection.State == HubConnectionState.Connected)
                {
                    _logger.LogInformation($"<== SynchronizeHub connected: {Terminal.Id} <-> {connection.ConnectionId}");

                    await GetTickerAsync();
                    await GetTradingTermsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void ReceiveTicker(TickerDto ticker)
        {
            if (Tickers.ContainsKey(ticker.market))
            {
                if (Tickers[ticker.market].trade_price == ticker.trade_price) { return; }

                Tickers[ticker.market].trade_price = ticker.trade_price;
            }
            else
            {
                Tickers.Add(ticker.market, ticker);
            }
        }

        public async Task GetTickerAsync()
        {
            try
            {
                var result = await connection.InvokeAsync<IEnumerable<TickerDto>>(HubMethod.GetTickers);

                if (result is not null || result.Any())
                {
                    foreach (var ticker in result)
                    {
                        ReceiveTicker(ticker);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task GetTradingTermsAsync()
        {
            if (!IsConnected) { return; }

            try
            {
                var bot = new MiningBotDto()
                {
                    Id = Terminal.Id,
                    Version = Terminal.Version,
                    MachineName = Terminal.MachineName,
                    CurrentDirectory = Terminal.CurrentDirectory,
                };

                TradingTermsDto result = null;
#if DEBUG
                var userId = "94b91906-75a3-47fc-9e77-38f27fdc5c24";

                result = await connection.InvokeAsync<TradingTermsDto>(HubMethod.GetTestTradingTerms, bot, userId);
#else
                result =  await connection.InvokeAsync<TradingTermsDto>(HubMethod.GetTradingTerms, bot);
#endif

                if (result is TradingTermsDto)
                {
                    if (Terminal.Ticket != result.Ticket || Depot.UserId != result.UserId)
                    {
                        Terminal.Ticket = result.Ticket;

                        _logger.LogInformation($"<== Received Id: {bot.Id} <-> {result.Ticket}: {result.UserId}");
                    }

                    Depot.SetTradingTerms(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task SaveOrdersAsync()
        {
            if (string.IsNullOrEmpty(Depot.TradingTerms?.UserId) || !IsConnected) { return; }

            try
            {
                IEnumerable<OrderDto> orders = Depot.CompletedOrders;

                IEnumerable<IEnumerable<OrderDto>> bundle = null;

                bundle = orders.OrderBy(f => f.created_at)
                               .Select((x, i) => new { Index = i, Value = x })
                               .GroupBy(x => x.Index / 50)
                               .Select(x => x.Select(v => v.Value));

                foreach (var items in bundle)
                {
                    _ = await connection.InvokeAsync<int>(HubMethod.SaveOrders, Depot.TradingTerms.UserId, items, true);
                }

                orders = Depot.WaitingOrders;

                bundle = orders.OrderBy(f => f.created_at)
                               .Select((x, i) => new { Index = i, Value = x })
                               .GroupBy(x => x.Index / 50)
                               .Select(x => x.Select(v => v.Value));

                foreach (var items in bundle)
                {
                    _ = await connection.InvokeAsync<int>(HubMethod.SaveOrders, Depot.TradingTerms.UserId, items, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task SavePositionsAsync()
        {
            if (string.IsNullOrEmpty(Depot.TradingTerms?.UserId) || !IsConnected) { return; }

            try
            {
                var bytes = await CompressAsync(Depot.Positions);

                var result = await connection.InvokeAsync<int>(HubMethod.SavePositions, Depot.TradingTerms.UserId, bytes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            async Task<byte[]> CompressAsync<T>(T obj)
            {
                using var dataStream = new MemoryStream();
                using var dataStreamWriter = new StreamWriter(dataStream);

                //Serialize object to MemoryStream
                await dataStreamWriter.WriteAsync(JsonSerializer.Serialize(obj));
                await dataStreamWriter.FlushAsync();

                //using var dataStream = new MemoryStream(JsonSerializer.Serialize(obj));

                //Ensure we reset to the begining of the stream to be able to read from it.
                dataStream.Position = 0;

                using var dataStreamCompressed = new MemoryStream();
                //Initialize the compression steam that will do the compression work.
                //Use the destination stream as a constructor parameter, and specify the desired compression level.
                using var dataStreamCompression = new GZipStream(dataStreamCompressed, CompressionMode.Compress, leaveOpen: true);

                //Copy the source data strean, to the compression stream. Once this operation is complete the `dataStreamCompressed` will contain the compressed version of the srouce stream
                await dataStream.CopyToAsync(dataStreamCompression);
                await dataStreamCompression.FlushAsync();

                var bytes = dataStreamCompressed.ToArray();

                return bytes;
            }
        }

        public async Task SaveTransfersAsync()
        {
            if (string.IsNullOrEmpty(Depot.TradingTerms?.UserId) || !IsConnected) { return; }

            List<TransferDto> tranfers = new List<TransferDto>();

            tranfers.AddRange(Depot.Deposits);
            tranfers.AddRange(Depot.Withdraws);

            try
            {
                var bundle = tranfers.OrderBy(f => f.created_at)
                                     .Select((x, i) => new { Index = i, Value = x })
                                     .GroupBy(x => x.Index / 50)
                                     .Select(x => x.Select(v => v.Value));

                foreach (var items in bundle)
                {
                    var result = await connection.InvokeAsync<int>(HubMethod.SaveTransfers, Depot.TradingTerms.UserId, items);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task OccurredFatalErrorAsync(string code, string message)
        {
            if (string.IsNullOrEmpty(Depot.TradingTerms?.UserId) || !IsConnected) { return; }

            await connection.InvokeAsync<bool>(HubMethod.OccurredFatalError, Depot.UserId, message);
        }

        private bool IsConnected => connection?.State == HubConnectionState.Connected ? true : false;

        private static class HubMethod
        {
            public static readonly string GetTickers = "GetTickers";
            public static readonly string ReceiveTicker = "ReceiveTicker";
            public static readonly string GetTradingTerms = "GetTradingTerms";
            public static readonly string GetTestTradingTerms = "GetTestTradingTerms";
            public static readonly string SavePositions = "SavePositions";
            public static readonly string SaveOrders = "SaveOrders";
            public static readonly string SaveTransfers = "SaveTransfers";
            public static readonly string OccurredFatalError = "OccurredFatalError";
        }
    }
}
