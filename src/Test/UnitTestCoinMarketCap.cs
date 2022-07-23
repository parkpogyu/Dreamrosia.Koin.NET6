using Dreamrosia.Koin.CoinMarketCap.Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class UnitTestCoinMarketCap
    {
        private ILogger _logger;

        public UnitTestCoinMarketCap()
        {

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                     .AddEnvironmentVariables()
                                                                     .Build();

            _logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            //Dreamrosia.Koin.CoinMarketCap.Infrastructure.Logger.SetLogger(_logger);
        }


        #region QUOTATION API
        [TestMethod]
        public async Task TestQtSymbols()
        {
            QtSymbol QtSymbol = new QtSymbol();

            var result = await QtSymbol.GetSymbolsAsync();

            if (result.Succeeded)
            {
                // _logger.Debug($"\n{ObjectDumper.Dump(result.Data, DumpStyle.CSharp)}");

                var group = result.Data.GroupBy(g => g.symbol).Where(f => f.Count() > 1);

                foreach (var g in group)
                {
                    foreach (var symbol in g)
                    {
                        _logger.Debug($"\n{ObjectDumper.Dump(symbol, DumpStyle.CSharp)}");
                    }
                }
            }
            else
            {
                _logger.Debug(ObjectDumper.Dump(result, DumpStyle.CSharp));
            }
        }

        [TestMethod]
        public async Task TestQtCandles()
        {
            QtCandle QtCandle = new QtCandle();

            DateTime now = DateTime.Now;

            var parameter = new QtCandle.QtParameter()
            {
                id = 52,
            };

            parameter.timeStart = ((DateTimeOffset)now.Date.AddDays(-1)).ToUnixTimeSeconds();
            parameter.timeEnd = ((DateTimeOffset)now).ToUnixTimeSeconds();

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

        #endregion
    }
}
