using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Hubs;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Constants.Coin;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Hubs
{
    public class SynchronizeHub : Hub<ISynchronizeClient>
    {
        private readonly IMiningBotService _miningBotService;
        private readonly IOrderService _orderService;
        private readonly IPositionService _positionService;
        private readonly ITransferService _transferService;
        private readonly IUPbitTickerService _upbitTickerService;
        private readonly IUPbitKeyService _upbitKeyService;
        private readonly ILogger<SynchronizeHub> _logger;

        private static readonly ConcurrentDictionary<string, string> connections = new ConcurrentDictionary<string, string>();

        public SynchronizeHub(IMiningBotService miningBotService,
                              IOrderService orderService,
                              IPositionService positionService,
                              ITransferService transferService,
                              IUPbitTickerService upbitTickerService,
                              IUPbitKeyService upbitKeyService,
                              ILogger<SynchronizeHub> logger)
        {
            _miningBotService = miningBotService;
            _orderService = orderService;
            _positionService = positionService;
            _transferService = transferService;
            _upbitTickerService = upbitTickerService;
            _upbitKeyService = upbitKeyService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var key = Context.GetHttpContext().Request.Query.SingleOrDefault(f => f.Key.Equals("guid", StringComparison.OrdinalIgnoreCase));

            var connectionId = Context.ConnectionId;

            if (key.Key is not null)
            {
                connections.TryAdd(connectionId, key.Value);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            string value;

            connections.TryGetValue(connectionId, out value);

            if (!string.IsNullOrEmpty(value))
            {
                connections.TryRemove(new KeyValuePair<string, string>(connectionId, value));
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<IEnumerable<TickerDto>> GetTickers()
        {
            var result = await _upbitTickerService.GetTradePricesAsync();

            return result.Data;
        }

        public async Task<TradingTermsExtensionDto> GetTradingTerms(MiningBotDto model)
        {
            var result = await _miningBotService.GetTradingTermsAsync(model);

            return result.Data;
        }

        public async Task<TradingTermsExtensionDto> GetTestTradingTerms(MiningBotDto model, string userId)
        {
            var result = await _miningBotService.GetTestTradingTermsAsync(model, userId);

            return result.Data;
        }

        public async Task<int> SaveOrders(string userId, IEnumerable<OrderDto> models, bool done)
        {
            var result = await _orderService.SaveOrdersAsync(userId, models, done);

            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _logger.LogError($"SaveOrder Error: userId:{userId}: {message}");
                }
            }

            return result.Data;
        }

        public async Task<int> SavePositions(string userId, byte[] bytes)
        {
            var models = await ObjectGZip.DecompressAsync<IEnumerable<PositionDto>>(bytes);

            var result = await _positionService.SavePositionsAsync(userId, models);

            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _logger.LogError($"SavePositons Error: userId:{userId}: {message}");
                }
            }

            var tickers = (await _upbitTickerService.GetTradePricesAsync()).Data;

            var items = (from pos in models
                         from tik in tickers.Where(f => f.market.Equals(pos.market)).DefaultIfEmpty()
                         select ((Func<PositionDto>)(() =>
                         {
                             pos.IsListed = tik is null ? false : true;
                             pos.trade_price = tik is null ? pos.trade_price : tik.trade_price;

                             return pos;
                         }))()).ToArray();

            var positions = new PositionsDto()
            {
                KRW = items.SingleOrDefault(f => f.code.Equals(Currency.Unit.KRW)),
                Coins = items.Where(f => !f.code.Equals(Currency.Unit.KRW))
            };

            var connectionIds = connections.Where(f => f.Value.Equals(userId));

            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId.Key)?.ReceivePositions(userId, positions);
            }

            return result.Data;
        }

        public async Task<int> SaveTransfers(string userId, IEnumerable<TransferDto> models)
        {
            var result = await _transferService.SaveTransfersAsync(userId, models);

            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _logger.LogError($"SaveTransfers Error: userId:{userId}: {message}");
                }
            }

            return result.Data;
        }

        public async Task<bool> OccurredFatalError(string userId, string error)
        {
            var result = await _upbitKeyService.OccurredFatalErrorAsync(userId, error);

            return result.Succeeded;
        }
    }
}