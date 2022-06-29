using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Extensions;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class BackTestingService : IBackTestingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISymbolService _symbolService;
        private readonly ILogger<AssetService> _logger;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public BackTestingService(IServiceProvider serviceProvider,
                                  ISymbolService symbolService,
                                  ILogger<AssetService> logger,
                                  IMapper mapper,
                                  IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _serviceProvider = serviceProvider;
            _symbolService = symbolService;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IResult<byte[]>> GetBackTestingAsync(BackTestingRequestDto model)
        {
            var markets = (await _symbolService.GetSymbolCodesAsync()).Data;

            IEnumerable<string> candidates = null;

            candidates = (from lt in markets
                          from rt in model.ChosenSymbols.Where(f => f.Equals(lt)).DefaultIfEmpty()
                          where rt is not null
                          select lt).ToArray();

            candidates = candidates.Any() ? candidates : markets;

            var first = new DateTime().ToUniversalDate();
            var today = DateTime.UtcNow.Date;

            List<IBackTestingTraderService> traders = new List<IBackTestingTraderService>();

            foreach (var code in candidates)
            {
                using var scope = _serviceProvider.CreateScope();

                var trader = scope.ServiceProvider.GetRequiredService<IBackTestingTraderService>();

                await trader.Prepare(model, code, first, today);

                traders.Add(trader);
            }

            var head = traders.Min(f => f.GetFirstSignalDate(model.TimeFrame));
            var rear = Convert.ToDateTime(model.RearDate).ToUniversalDate();
            var startDate = Convert.ToDateTime(model.HeadDate).ToUniversalDate();

            head = head > startDate ? head : startDate;

            var dates = DateTimeExtensions.GetDays(head, (int)rear.Subtract(head).TotalDays + 1);
            var count = candidates.Count();

            List<AssetDto> assets = new List<AssetDto>();

            AssetDto asset = new AssetDto()
            {
                Deposit = model.SeedMoney,
                InvsAmt = model.SeedMoney,
            };

            foreach (var date in dates)
            {
                model.Amount = (long)AdjustBidAmount(asset.DssAmt, count);

                Parallel.ForEach(traders, trader =>
                {
                    trader.Simulate(date, model.TimeFrame);
                });

                asset = MakeAsset(date, traders, asset);

                assets.Add(asset);
            }

            BackTestingReportDto report = new BackTestingReportDto();

            report.Assets = assets;
            report.InvsAmt = model.SeedMoney;

            var orders = traders.SelectMany(f => f.Orders).OrderBy(f => f.created_at);

            if (orders.Any())
            {
                report.MakeReport(orders);
            }
            else
            {
                report.HeadDate = dates.First().Date;
            }

            report.RearDate = dates.Last();

            report.Orders = model.IncludeOrders ? orders : new List<PaperOrderDto>();
            report.Positions = model.IncludePositons ? traders.SelectMany(f => f.Positions.Where(f => f.balance > 0))
                                                              .OrderBy(f => f.created_at) : new List<PaperPositionDto>();

            if (report.Positions.Any())
            {
                foreach (var position in report.Positions)
                {
                    position.IsListed = true;
                }
            }

            var compressed = await ObjectGZip.CompressAsync(report);

            return await Result<byte[]>.SuccessAsync(compressed);

            double AdjustBidAmount(double total, int count)
            {
                bool applyMarket = model.ApplyMarketPrice;

                double amount = 0;

                if (model.AmountOption == BidAmountOption.Fixed)
                {
                    amount = model.Amount;
                }
                else
                {
                    double cutOffUnit = 1000D; // 절사 단위
                    double MaxBidAmount = 1000000000D; // KRW 최대 주문 금액: 1,000,000,000

                    model.AmountRate = model.AmountOption == BidAmountOption.Auto ? 100F / count : model.AmountRate;

                    amount = (long)(total * (model.AmountRate / 100F));
                    amount = (amount / cutOffUnit) * cutOffUnit;  // 천원 단위로 거래

                    if (amount < model.Minimum)
                    {
                        amount = model.Minimum;
                    }
                    else if (model.Maximum < amount)
                    {
                        amount = model.Maximum == 0 ? amount : model.Maximum;
                        amount = amount > MaxBidAmount ? MaxBidAmount : amount;
                    }
                }

                return amount;
            }

            AssetDto MakeAsset(DateTime date, IEnumerable<IBackTestingTraderService> traders, AssetDto prev)
            {
                var positions = traders.Where(f => f.Position.created_at == date && f.Position.balance > 0)
                                       .Select(f => f.Position)
                                       .ToArray();

                var orders = traders.SelectMany(f => f.Orders.Where(o => o.created_at == date)).ToArray();

                var bids = orders.Where(f => f.side == OrderSide.bid).ToArray();
                var asks = orders.Where(f => f.side == OrderSide.ask).ToArray();

                AssetDto asset = new AssetDto()
                {
                    InvsAmt = prev.InvsAmt,
                    created_at = date,
                };

                asset.AskAmt = asks.Sum(f => f.exec_amount);
                asset.BidAmt = bids.Sum(f => f.exec_amount);
                asset.Fee = asks.Sum(f => Convert.ToDouble(f.paid_fee)) + bids.Sum(f => Convert.ToDouble(f.paid_fee));
                asset.PnL = asks.Sum(f => f.PnL);
                asset.BalEvalAmt = positions.Sum(f => f.BalEvalAmt);
                asset.Deposit = prev.Deposit - (asset.BidAmt + asset.Fee) + asset.AskAmt;

                if (asset.Deposit < 0)
                {
                    asset.InAmt = Math.Abs(asset.Deposit);
                    asset.Deposit = 0;

                    asset.BorrowedAmt = prev.BorrowedAmt + asset.InAmt;
                }
                else if (0 < prev.BorrowedAmt && prev.BorrowedAmt < asset.Deposit)
                {
                    asset.OutAmt = prev.BorrowedAmt;
                    asset.Deposit = asset.Deposit - prev.BorrowedAmt;
                    asset.BorrowedAmt = 0;
                }
                else
                {
                    asset.BorrowedAmt = prev.BorrowedAmt;
                }

                asset.PositionCount = positions.Count();

                foreach (var trader in traders)
                {
                    if (trader.Position.IsCleared)
                    {
                        trader.Position.balance = 0;
                    }
                }

                asset.MaxDssAmt = asset.DssAmt > prev.MaxDssAmt ? asset.DssAmt : prev.MaxDssAmt;
                asset.MaxInvsPnL = asset.InvsPnL > prev.MaxInvsPnL ? asset.InvsPnL : prev.MaxInvsPnL;

                return asset;
            }
        }
    }
}