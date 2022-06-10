using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Collections.Generic;
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
                access_key = "lNTMHCHx60Fldox9Y3mQYSoKWL8X9k1yxcJDrvXy",
                secret_key = "CmZnkrz2Uh8GoBiQwcNQk1xpJPQpxuZofx46ivKD",
            });

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                     .AddEnvironmentVariables()
                                                                     .Build();

            _logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            Dreamrosia.Koin.UPbit.Infrastructure.Logger.SetLogger(_logger);
        }

        [TestMethod]
        public async Task TestQtCandle()
        {
            QtCandle QtCandle = new QtCandle();

            QtCandle.QtParameter parameter = new QtCandle.QtParameter()
            {
                market = "KRW-SAND",
                to = null,
            };

            var result = await QtCandle.GetCandlesAsync(parameter);

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Candle>())
                {
                    _logger.Debug($"{item.candle_date_time_utc:d} : {item.trade_price:N4}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestQtCrix()
        {
            QtCrix QtCrix = new QtCrix();

            var result = await QtCrix.GetCrixesAsync();

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Crix>())
                {
                    _logger.Debug($"{item.crix_code} : {item.price:N4}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }


        [TestMethod]
        public async Task TestQtSymbol()
        {
            QtSymbol QtSymbol = new QtSymbol();

            var result = await QtSymbol.GetSymbolsAsync();

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Symbol>())
                {
                    _logger.Debug($"{item.market} : {item.korean_name}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestExAcessKeys()
        {
            ExAccessKeys ExAccessKeys = new ExAccessKeys();

            var result = await ExAccessKeys.GetAccessKeysAsync();

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.UPbitKey>())
                {
                    _logger.Debug($"{item.access_key} : {Convert.ToDateTime(item.expire_at):s}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestExPositinos()
        {
            ExPositions ExPositions = new ExPositions();

            var result = await ExPositions.GetPositionsAsync();

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Position>())
                {
                    _logger.Debug($"{item.unit_currency}-{item.code} : {item.balance + item.locked}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestDoneOrders()
        {
            ExOrders ExOrders = new ExOrders();

            var result = await ExOrders.GetCompletedOrdersAsync(new DateTime(2022, 05, 30));

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Order>())
                {
                    _logger.Debug($"{item.created_at:s}: {item.market}, {item.side}, {item.executed_volume}, {item.trades_count}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestWaitOrders()
        {
            ExOrders ExOrders = new ExOrders();

            var result = await ExOrders.GetWaitingOrdersAsync();

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Order>())
                {
                    _logger.Debug($"{item.created_at:s}: {item.market}, {item.side}, {item.executed_volume}, {item.trades_count}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestDepositTransfers()
        {
            ExTransfers ExTransfers = new ExTransfers();

            var result = await ExTransfers.GetDepositTransfersAsync(new DateTime(2022, 05, 18));

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Transfer>())
                {
                    _logger.Debug($"{item.done_at:s}: {item.code}, {item.amount}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

        [TestMethod]
        public async Task TestWithdrawTransfers()
        {
            ExTransfers ExTransfers = new ExTransfers();

            var result = await ExTransfers.GetWithdrawTransfersAsync(new DateTime(2021, 10, 26));

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.Transfer>())
                {
                    _logger.Debug($"{item.done_at:s}: {item.code}, {item.amount}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
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

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));

            parameter.uuid = "5ea41dd6-1b51-4b24-a21c-5176222df130";
            parameter.type = TransferType.withdraw;

            result = await ExTransfer.GetTransferAsync(parameter);

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
        }

        [TestMethod]
        public async Task TestOrderPost()
        {
            ExOrderPost ExOrderPost = new ExOrderPost();

            ExOrderPost.ExParameter parameter = new ExOrderPost.ExParameter()
            {
                market = "KRW-BTC",
                side = OrderSide.bid,
                volume = 1.1,
                price = 1000.2,
                ord_type = OrderType.limit,
            };

            var result = await ExOrderPost.OrderPostAsync(parameter);

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
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

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
        }

        [TestMethod]
        public async Task TestOrderAvailable()
        {
            ExOrderAvailable ExOrderAvailable = new ExOrderAvailable();

            ExOrderAvailable.ExParameter parameter = new ExOrderAvailable.ExParameter()
            {
                market = "KRW-BTC",
            };

            var result = await ExOrderAvailable.GetOrderAvailableAsync(parameter);

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
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

            _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
        }

        [TestMethod]
        public async Task TestCoinStatus()
        {
            ExCoinStatus ExCoinStatus = new ExCoinStatus();

            var result = await ExCoinStatus.GetCoinStatusesAsync();

            if (result.Succeeded)
            {
                foreach (var item in result.Data ?? new List<UPbitModels.CoinStatus>())
                {
                    _logger.Debug($"{item.code}: {item.wallet_state}, {item.block_state}");
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.Console));
            }
        }

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
        }
    }
}
