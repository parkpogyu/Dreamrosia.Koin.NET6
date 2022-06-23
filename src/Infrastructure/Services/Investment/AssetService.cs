﻿using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Constants.Coin;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Extensions;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class AssetService : IAssetService
    {
        private readonly BlazorHeroContext _context;
        private readonly ICandleService _candleService;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public AssetService(BlazorHeroContext context,
                            ICandleService candleService,
                            IMapper mapper,
                            ILogger<AssetService> logger,
                            IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _context = context;
            _candleService = candleService;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IResult<AssetReportDto>> GetAssetsAsync(string userId)
        {

            try
            {
                var code = await _context.UserLogins
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(f => f.ProviderKey.Equals(userId));
                if (code is not null)
                {
                    userId = code.UserId;
                }

                AssetReportDto report = new AssetReportDto();

                var orders = _context.Orders
                                     .AsNoTracking()
                                     .Where(f => f.UserId.Equals(userId))
                                     .ToArray();

                var transfers = _context.Transfers
                                        .AsNoTracking()
                                        .Where(f => f.UserId.Equals(userId))
                                        .ToArray();


                var _orders = _mapper.Map<IEnumerable<AssetTrackingDto>>(orders);
                var _transfers = _mapper.Map<IEnumerable<AssetTrackingDto>>(transfers);

                List<AssetTrackingDto> trackings = new List<AssetTrackingDto>();

                bool mixedOrders = false;

                if (_orders.Count(f => !f.market.StartsWith(Currency.Unit.KRW)) > 0)
                {
                    mixedOrders = true;
                }
                else
                {
                    trackings.AddRange(_orders);
                }

                trackings.AddRange(_transfers);

                if (!trackings.Any()) { return await Result<AssetReportDto>.SuccessAsync(report); }

                Dictionary<string, PaperPositionDto> DicPositions = new Dictionary<string, PaperPositionDto>();
                Dictionary<string, IEnumerable<CandleDto>> DicCandles = new Dictionary<string, IEnumerable<CandleDto>>();

                var symbols = _context.Candles.AsNoTracking()
                                              .GroupBy(g => g.market)
                                              .Select(f => f.Key)
                                              .AsEnumerable();

                var markets = (from lt in symbols
                               from rt in trackings.Where(f => lt.Equals(f.market)).DefaultIfEmpty()
                               where rt is not null
                               select lt).Distinct().ToArray();

                DateTime today = DateTime.UtcNow.Date;

                foreach (var market in markets)
                {
                    var splits = market.Split("-", StringSplitOptions.RemoveEmptyEntries);

                    DicPositions.Add(market, new PaperPositionDto()
                    {
                        unit_currency = splits[0],
                        code = splits[1]
                    }); ;

                    var tracking = trackings.Where(f => market.Equals(f.market))
                                            .OrderBy(f => f.done_at)
                                            .FirstOrDefault();

                    if (tracking is not null)
                    {
                        var candles = await _candleService.GetCandlesAsync(market,
                                                                           tracking.done_at.ToUniversalDate(),
                                                                           today);

                        DicCandles.Add(market, candles.Data);
                    }
                }

                List<AssetDto> assets = new List<AssetDto>();
                AssetDto asset = null;

                List<PaperPositionDto> positions = new List<PaperPositionDto>();
                PaperPositionDto position = null;

                CandleDto candle = null;

                double balance = 0, volume = 0, deposit = 0, investment = 0;

                DateTime head = trackings.Min(f => f.done_at).ToUniversalDate();

                var dates = head.GetDays((int)today.Date.Subtract(head).TotalDays + 1).ToArray();

                try
                {
                    foreach (var date in dates)
                    {
                        var trackingsInDate = trackings.Where(f => f.done_at.ToUniversalDate() == date)
                                                       .OrderBy(f => f.done_at)
                                                       .ThenByDescending(f => f.side)
                                                       .ToArray();

                        asset = new AssetDto()
                        {
                            Deposit = deposit,
                            created_at = date
                        };

                        foreach (var tracking in trackingsInDate)
                        {
                            // Order
                            if (tracking.IsOrder)
                            {
                                position = DicPositions[tracking.market];

                                volume = tracking.executed_volume;

                                if (tracking.side == OrderSide.ask)
                                {
                                    position.balance = Math.Round(position.balance - volume, 8);

                                    tracking.SetPnL(position.avg_buy_price);

                                    deposit = deposit + tracking.exec_amount - tracking.paid_fee;

                                    asset.AskAmt = asset.AskAmt + tracking.exec_amount;
                                    asset.Fee = asset.Fee + tracking.paid_fee;
                                    asset.PnL = asset.PnL + tracking.PnL;
                                }
                                else if (tracking.side == OrderSide.bid)
                                {
                                    balance = position.balance + volume;

                                    position.avg_buy_price = (position.PchsAmt + tracking.exec_amount) / balance;
                                    position.balance = balance;
                                    position.created_at = tracking.done_at;

                                    deposit = deposit - tracking.exec_amount - tracking.paid_fee;

                                    asset.BidAmt = asset.BidAmt + tracking.exec_amount;
                                    asset.Fee = asset.Fee + tracking.paid_fee;
                                }
                            }
                            // Transfer
                            else
                            {
                                if (tracking.code.Equals(Currency.Unit.KRW))
                                {
                                    if (tracking.type == TransferType.deposit)
                                    {
                                        deposit = deposit + tracking.amount;
                                        investment = investment + tracking.amount;

                                        asset.InAmt = asset.InAmt + tracking.amount;
                                    }
                                    else
                                    {
                                        deposit = deposit - tracking.amount - tracking.fee;
                                        investment = investment - tracking.amount - tracking.fee;
                                        //investment = investment < 0 ? 0 : investment;

                                        asset.OutAmt = asset.OutAmt + tracking.amount + tracking.fee;
                                    }
                                }
                                else
                                {
                                    var market = tracking.market;

                                    if (!DicPositions.ContainsKey(market)) { continue; }

                                    position = DicPositions[market];

                                    volume = Convert.ToDouble(tracking.amount);

                                    if (tracking.TransferState == TransferState.done)
                                    {
                                        position.balance = position.balance - volume;
                                    }
                                    else if (tracking.TransferState == TransferState.accepted)
                                    {
                                        balance = position.balance + volume;

                                        position.avg_buy_price = position.PchsAmt / balance;
                                        position.balance = balance;
                                        position.created_at = tracking.done_at;
                                    }
                                }
                            }
                        }

                        if (!mixedOrders)
                        {
                            foreach (var item in DicPositions.Values)
                            {
                                if (item.balance > 0)
                                {
                                    if (item.balance < 0.00000001)
                                    {
                                        item.balance = 0;
                                        continue;
                                    }

                                    candle = DicCandles[item.market].SingleOrDefault(f => f.candle_date_time_utc == date);

                                    if (candle is null)
                                    {
                                        item.trade_price = item.avg_buy_price;
                                    }
                                    else
                                    {
                                        item.trade_price = candle.trade_price;
                                    }

                                    item.created_at = date;

                                    positions.Add(_mapper.Map<PaperPositionDto>(item));
                                }
                            }
                        }

                        asset.BalEvalAmt = DicPositions.Values.Where(f => f.balance > 0).Sum(f => f.BalEvalAmt);
                        asset.Deposit = Math.Truncate(deposit);
                        asset.InvsAmt = investment;

                        var last = assets.LastOrDefault();

                        if (last is null)
                        {
                            asset.MaxDssAmt = asset.DssAmt;
                            asset.MaxInvsPnL = asset.MaxInvsPnL;
                        }
                        else
                        {
                            asset.MaxDssAmt = asset.DssAmt > last.MaxDssAmt ? asset.DssAmt : last.MaxDssAmt;
                            asset.MaxInvsPnL = asset.InvsPnL > last.MaxInvsPnL ? asset.InvsPnL : last.MaxInvsPnL;
                        }

                        assets.Add(asset);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }

                report.Assets = assets;
                report.InvsAmt = investment;

                var torders = trackings.Where(f => f.IsOrder);

                report.MakeReport(_mapper.Map<IEnumerable<PaperOrderDto>>(torders));
                report.RearDate = dates.Max();

                var remains = DicPositions.Values.Where(f => f.balance != 0);

                return await Result<AssetReportDto>.SuccessAsync(report);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<AssetReportDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}