using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Test
{
    [TestClass]
    public class UnitTestUPbit
    {
        private ILogger _logger;

        public UnitTestUPbit()
        {
            ExchangeClientKeys.SetAuthenticationKey(new UPbitModels.UPbitKey()
            {
                access_key = "HWQRJZkh6N8G5zg87fT0TkyTxjxPXiVlXNcPuO1s",
                secret_key = "ix4bPRV9xt6rerDzBwpW6hsSzVXMRMurxdKrT8Pc",
            });

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                     .AddEnvironmentVariables()
                                                                     .Build();

            _logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            Dreamrosia.Koin.UPbit.Infrastructure.Logger.SetLogger(_logger);
        }

        #region EXCHANGE API
        [TestMethod]
        public async Task TestExPositinos()
        {
            ExPositions ExPositions = new ExPositions();

            var result = await ExPositions.GetPositionsAsync();

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestOrderAvailable()
        {
            ExOrderAvailable ExOrderAvailable = new ExOrderAvailable();

            ExOrderAvailable.ExParameter parameter = new ExOrderAvailable.ExParameter();

            QtSymbol QtSymbol = new QtSymbol();

            var symbols = (await QtSymbol.GetSymbolsAsync()).Data ?? new List<UPbitModels.Symbol>();

            foreach (var symbol in symbols)
            {
                parameter.market = symbol.market;

                var result = await ExOrderAvailable.GetOrderAvailableAsync(parameter);

                if (result.Succeeded)
                {
                    _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
                }
                else
                {
                    _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
                }
            }
        }

        [TestMethod]
        public async Task TestDoneOrders()
        {
            ExOrders ExOrders = new ExOrders();

            var result = await ExOrders.GetCompletedOrdersAsync(new DateTime(2022, 05, 30));

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");

                if (result.Data.Any())
                {
                    var order = result.Data.Last();

                    ExOrder ExOrder = new ExOrder();

                    ExOrder.ExParameter parameter = new ExOrder.ExParameter()
                    {
                        uuid = order.uuid,
                    };

                    var detail = await ExOrder.GetOrderAsync(parameter);

                    if (detail.Succeeded)
                    {
                        _logger.Debug($"\n{ObjectDumper.Dump(detail.Data, DumpStyle.CSharp)}");
                    }
                    else
                    {
                        _logger.Debug(ObjectDumper.Dump(detail, DumpStyle.CSharp));
                    }
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestWaitOrders()
        {
            ExOrders ExOrders = new ExOrders();

            var result = await ExOrders.GetWaitingOrdersAsync();

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");

                if (result.Data.Any())
                {
                    var order = result.Data.Last();

                    ExOrder ExOrder = new ExOrder();

                    ExOrder.ExParameter parameter = new ExOrder.ExParameter()
                    {
                        uuid = order.uuid,
                    };

                    var detail = await ExOrder.GetOrderAsync(parameter);

                    if (detail.Succeeded)
                    {
                        _logger.Debug($"\n{ObjectDumper.Dump(detail.Data, DumpStyle.CSharp)}");
                    }
                    else
                    {
                        _logger.Debug(ObjectDumper.Dump(detail, DumpStyle.CSharp));
                    }
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestOrder()
        {
            ExOrder ExOrder = new ExOrder();

            ExOrder.ExParameter parameter = new ExOrder.ExParameter()
            {
                uuid = "92db920c-aea3-417d-bc94-15dd9deaf20b"
            };

            var result = await ExOrder.GetOrderAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestOrderDelete()
        {
            ExOrderDelete ExOrderDelete = new ExOrderDelete();

            ExOrderDelete.ExParameter parameter = new ExOrderDelete.ExParameter()
            {
                uuid = "KRW-BTC",
                indendifier = "KRW-BTC",
            };

            var result = await ExOrderDelete.OrderDeleteAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestOrderPost()
        {
            ExOrderPost ExOrderPost = new ExOrderPost();

            ExOrderPost.ExParameter parameter = new ExOrderPost.ExParameter()
            {
                market = "KRW-BTC",
                side = OrderSide.bid,
                price = (decimal)1000.2,
                ord_type = OrderType.price,
            };

            var result = await ExOrderPost.OrderPostAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestDepositTransfers()
        {
            ExTransfers ExTransfers = new ExTransfers();

            var result = await ExTransfers.GetDepositTransfersAsync(new DateTime(2022, 05, 18));

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");

                if (result.Data.Any())
                {
                    var transfer = result.Data.Last();

                    ExTransfer ExTransfer = new ExTransfer();

                    ExTransfer.ExParameter parameter = new ExTransfer.ExParameter()
                    {
                        uuid = transfer.uuid,
                        type = TransferType.deposit,
                    };

                    var detail = await ExTransfer.GetTransferAsync(parameter);

                    if (detail.Succeeded)
                    {
                        _logger.Debug($"\n{ObjectDumper.Dump(detail.Data, DumpStyle.CSharp)}");
                    }
                    else
                    {
                        _logger.Debug(ObjectDumper.Dump(detail, DumpStyle.CSharp));
                    }
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestWithdrawTransfers()
        {
            ExTransfers ExTransfers = new ExTransfers();

            var result = await ExTransfers.GetWithdrawTransfersAsync(new DateTime(2021, 10, 26));

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");

                if (result.Data.Any())
                {
                    var transfer = result.Data.Last();

                    ExTransfer ExTransfer = new ExTransfer();

                    ExTransfer.ExParameter parameter = new ExTransfer.ExParameter()
                    {
                        uuid = transfer.uuid,
                        type = TransferType.withdraw,
                    };

                    var detail = await ExTransfer.GetTransferAsync(parameter);

                    if (detail.Succeeded)
                    {
                        _logger.Debug($"\n{ObjectDumper.Dump(detail.Data, DumpStyle.CSharp)}");
                    }
                    else
                    {
                        _logger.Debug(ObjectDumper.Dump(detail, DumpStyle.CSharp));
                    }
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestTransfer()
        {
            ExTransfer ExTransfer = new ExTransfer();

            ExTransfer.ExParameter parameter = new ExTransfer.ExParameter()
            {
                uuid = "05c8ed95-3f80-43b4-abcf-9097edc1e865",
                type = TransferType.deposit,
            };

            var result = await ExTransfer.GetTransferAsync(parameter);

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));

            parameter.uuid = "5ea41dd6-1b51-4b24-a21c-5176222df130";
            parameter.type = TransferType.withdraw;

            result = await ExTransfer.GetTransferAsync(parameter);

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
        }

        [TestMethod]
        public async Task TestWithdrawAvailable()
        {
            QtSymbol QtSymbol = new QtSymbol();

            var symbols = (await QtSymbol.GetSymbolsAsync()).Data ?? new List<UPbitModels.Symbol>();

            ExWithdrawAvailable ExWithdrawAvailable = new ExWithdrawAvailable();

            ExWithdrawAvailable.ExParameter parameter = new ExWithdrawAvailable.ExParameter();

            foreach (var symbol in symbols)
            {
                parameter.currency = symbol.code;

                var available = await ExWithdrawAvailable.GetWithdrawAvailableAsync(parameter);

                if (available.Succeeded)
                {
                    _logger.Debug($"\n{ObjectDumper.Dump(available.Data, DumpStyle.CSharp)}");
                }
                else
                {
                    _logger.Debug(ObjectDumper.Dump(available, DumpStyle.CSharp));
                }
            }
        }

        [TestMethod]
        public async Task TestCoinStatus()
        {
            ExCoinStatus ExCoinStatus = new ExCoinStatus();

            var result = await ExCoinStatus.GetCoinStatusesAsync();

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestExAcessKeys()
        {
            ExAccessKeys ExAccessKeys = new ExAccessKeys();

            var result = await ExAccessKeys.GetAccessKeysAsync();

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }
        #endregion

        #region QUOTATION API
        [TestMethod]
        public async Task TestQtSymbol()
        {
            QtSymbol QtSymbol = new QtSymbol();

            var result = await QtSymbol.GetSymbolsAsync();

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestQtCandle()
        {
            QtCandle QtCandle = new QtCandle();

            QtCandle.QtParameter parameter = new QtCandle.QtParameter()
            {
                market = "KRW-ARDR",
                to = null,
            };

            var result = await QtCandle.GetCandlesAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestQtCandles()
        {
            QtSymbol QtSymbol = new QtSymbol();

            var symbols = await QtSymbol.GetSymbolsAsync();

            if (symbols.Succeeded)
            {
                QtCandle QtCandle = new QtCandle();

                foreach (var symbol in symbols.Data)
                {
                    QtCandle.QtParameter parameter = new QtCandle.QtParameter()
                    {
                        market = symbol.market,
                        to = null,
                    };

                    var candles = await QtCandle.GetCandlesAsync(parameter);

                    if (candles.Succeeded)
                    {
                        _logger.Debug($"\n{ObjectDumper.Dump(candles.Data, DumpStyle.CSharp)}");
                    }
                    else
                    {
                        _logger.Debug(ObjectDumper.Dump(candles, DumpStyle.CSharp));
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestQtMarketTrade()
        {
            QtMarketTrade QtMarketTrade = new QtMarketTrade();

            QtMarketTrade.QtParameter parameter = new QtMarketTrade.QtParameter()
            {
                market = "KRW-BTC"
            };

            var result = await QtMarketTrade.GetMarketTradesAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestQtTicker()
        {
            QtTicker QtTicker = new QtTicker();

            QtTicker.QtParameter parameter = new QtTicker.QtParameter();

            parameter.markets.Add("KRW-BTC");

            var result = await QtTicker.GetTickersAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestQtOrderBook()
        {
            QtOrderBook QtOrderBook = new QtOrderBook();

            QtOrderBook.QtParameter parameter = new QtOrderBook.QtParameter();

            parameter.markets.Add("KRW-BTC");

            var result = await QtOrderBook.GetOrderBooksAsync(parameter);

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestQtCrix()
        {
            QtCrix QtCrix = new QtCrix();

            var result = await QtCrix.GetCrixesAsync();

            if (result.Succeeded)
            {
                _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }
        #endregion

        [TestMethod]
        public async Task TestBundle()
        {
            List<Task> tasks = new List<Task>();

            ExAccessKeys _exAccessKeys = new ExAccessKeys();
            ExOrders _exOrders = new ExOrders();
            ExOrderPost _exOrderPost = new ExOrderPost();
            ExPositions _exPositions = new ExPositions();
            ExTransfers _exDeposits = new ExTransfers();
            ExTransfers _exWithdraws = new ExTransfers();

            ExOrderPost.ExParameter parameter = new ExOrderPost.ExParameter()
            {
                market = "KRW-BTC",
                side = OrderSide.bid,
                volume = 1,
                price = 1,
                ord_type = OrderType.limit,
            };

            //var accessKeys = _exAccessKeys.GetAsync<IEnumerable<UPbitKey>>();
            //var orders = _exOrders.GetDoneOrdersAsync<IEnumerable<Order>>();
            //var orderPost = _exOrderPost.PostAsync<UPbitModels.Order>(parameter);
            //var positions = _exPositions.GetAsync<IEnumerable<Position>>();
            //var deposits = _exDeposits.GetDepositTransfersAsync<IEnumerable<Transfer>>();
            //var withdraws = _exWithdraws.GetWithdrawTransfersAsync<IEnumerable<Transfer>>();

            //tasks.Add(accessKeys);
            //tasks.Add(orderPost);
            //tasks.Add(orders);
            //tasks.Add(positions);
            //tasks.Add(deposits);
            //tasks.Add(withdraws);

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TestTruncate()
        {
            DateTime touched = new DateTime();
            TimeSpan? elapsed = null;

            //var pos = "136602727.27272727";
            var pos = "136602727.2727";

            var convert_dec = Convert.ToDecimal(pos);
            var convert_dob = convert_dec;

            System.Diagnostics.Debug.WriteLine($"pos: {pos}");
            System.Diagnostics.Debug.WriteLine($"convert dec: {convert_dec:N8}");
            System.Diagnostics.Debug.WriteLine($"convert dob: {convert_dob:N8}");

            convert_dob = convert_dob + .0001m;

            System.Diagnostics.Debug.WriteLine($"convert dob: {convert_dob:N8}");

            convert_dob = convert_dob - .0005m;

            System.Diagnostics.Debug.WriteLine($"convert dob: {convert_dob:N8}");

            var rate = (float)Ratio.ToPercentage(-259238, 13645555);

            var a = 22087623;

            var b = a * (1F / 113F);

            var c = (long)(b / 1000) * 1000;
        }
    }
}
