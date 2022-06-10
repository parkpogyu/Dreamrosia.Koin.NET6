using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class TestService : ITestService
    {
        private readonly IMapper _mapper;
        private readonly IUPbitCandleService _upbitCandleService;
        private readonly IUPbitCrixService _upbitCrixService;
        private readonly IUPbitKeyService _upbitKeyService;
        private readonly IUPbitSymbolService _upbitSymbolService;
        private readonly IOrderService _orderService;
        private readonly IPositionService _positionService;
        private readonly ISeasonSignalService _seasonSignalService;
        private readonly ITransferService _transferService;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public TestService(IMapper mapper,
                           IUPbitCandleService upbitCandleService,
                           IUPbitCrixService upbitCrixService,
                           IUPbitKeyService upbitKeyService,
                           IUPbitSymbolService upbitSymbolService,
                           ICandleService candleService,
                           IOrderService orderService,
                           ISeasonSignalService seasonSignalService,
                           ITransferService transferService,
                           IPositionService positionService,
                           IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _mapper = mapper;
            _upbitCandleService = upbitCandleService;
            _upbitCrixService = upbitCrixService;
            _upbitKeyService = upbitKeyService;
            _upbitSymbolService = upbitSymbolService;
            _orderService = orderService;
            _seasonSignalService = seasonSignalService;
            _transferService = transferService;
            _positionService = positionService;
            _localizer = localizer;
        }

        public async Task<IResult> GetUPbitCandlesAsync()
        {
            return await _upbitCandleService.GetCandlesAsync();
        }

        public async Task<IResult> GetUPbitCrixesAsync()
        {
            return await _upbitCrixService.GetCrixesAsync();
        }

        public async Task<IResult> GetUPbitSymbolsAsync()
        {
            return await _upbitSymbolService.GetSymbolsAsync();
        }

        public async Task<IResult> GetUPbitOrdersAsync(string userId)
        {
            var key = await _upbitKeyService.GetUPbitKeyAsync(userId);

            ExchangeClientKeys.SetAuthenticationKey(new UPbitModels.UPbitKey()
            {
                access_key = key.Data.access_key,
                secret_key = key.Data.secret_key,
            });

            ExOrders exOrders = new ExOrders();

            var last = await _orderService.GetLastOrderAsync(userId);

            if (last.Succeeded)
            {
                var result = await exOrders.GetCompletedOrdersAsync(last.Data?.created_at);

                if (result.Succeeded)
                {
                    var items = result.Data.Select(f =>
                    {
                        var item = _mapper.Map<OrderDto>(f);

                        item.UserId = userId;

                        return item;

                    });

                    if (items.Any())
                    {
                        await _orderService.SaveOrdersAsync(userId, items, done: true);
                    }

                    return await Result<IEnumerable<OrderDto>>.SuccessAsync(items);
                }
                else
                {
                    return await Result<IEnumerable<OrderDto>>.FailAsync(result.Messages);
                }
            }
            else
            {
                return await Result<IEnumerable<TransferDto>>.FailAsync(last.Messages);
            }
        }

        public async Task<IResult> GetUPbitTransfersAsync(string userId, TransferType type)
        {
            var key = await _upbitKeyService.GetUPbitKeyAsync(userId);

            ExchangeClientKeys.SetAuthenticationKey(new UPbitModels.UPbitKey()
            {
                access_key = key.Data.access_key,
                secret_key = key.Data.secret_key,
            });

            ExTransfers exTransfers = new ExTransfers();

            var last = await _transferService.GetLastTransferAsync(userId, type);

            if (last.Succeeded)
            {
                var result = type == TransferType.deposit ?
                            await exTransfers.GetDepositTransfersAsync(last.Data?.created_at) :
                            await exTransfers.GetWithdrawTransfersAsync(last.Data?.created_at);

                if (result.Succeeded)
                {
                    var items = result.Data.Select(f =>
                    {
                        var item = _mapper.Map<TransferDto>(f);

                        item.UserId = userId;

                        return item;
                    });

                    if (items.Any())
                    {
                        await _transferService.SaveTransfersAsync(userId, items);
                    }

                    return await Result<IEnumerable<TransferDto>>.SuccessAsync(items);
                }
                else
                {
                    return await Result<IEnumerable<TransferDto>>.FailAsync(result.Messages);
                }
            }
            else
            {
                return await Result<IEnumerable<TransferDto>>.FailAsync(last.Messages);
            }
        }

        public async Task<IResult> GetUPbitPositionsAsync(string userId)
        {
            var key = await _upbitKeyService.GetUPbitKeyAsync(userId);

            ExchangeClientKeys.SetAuthenticationKey(new UPbitModels.UPbitKey()
            {
                access_key = key.Data.access_key,
                secret_key = key.Data.secret_key
            });

            ExPositions exPositions = new ExPositions();

            var result = await exPositions.GetPositionsAsync();

            if (result.Succeeded)
            {
                var items = result.Data.Select(f =>
                {
                    var item = _mapper.Map<PositionDto>(f);

                    item.UserId = userId;

                    return item;
                });

                if (items.Any())
                {
                    await _positionService.SavePositionsAsync(userId, items);
                }

                return await Result<IEnumerable<PositionDto>>.SuccessAsync(items);
            }
            else
            {
                return await Result<IEnumerable<PositionDto>>.FailAsync(result.Messages);
            }
        }

        public async Task<IResult> GetSeasonSignalsAsync(string userId)
        {
            return await _seasonSignalService.GetSeasonSignalsAsync(userId);
        }

        public async Task<IResult> UpdateSeasonSignalsAsync(string userId)
        {
            return await _seasonSignalService.UpdateSeasonSignalsAsync(userId);
        }
    }
}