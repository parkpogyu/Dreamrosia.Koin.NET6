using AutoMapper;
using Dapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class UnitTestBackTesting
    {
        private ILogger _logger;
        private IMapper _mapper;

        private string ConnectionString => "Server=localhost;Database=fastlane;Uid=fastlane;Pwd=totoro79;";

        public UnitTestBackTesting()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                     .AddEnvironmentVariables()
                                                                     .Build();

            _logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            Dreamrosia.Koin.UPbit.Infrastructure.Logger.SetLogger(_logger);

            _mapper = new MapperConfiguration(c =>
            {
                c.AddProfile<CandleProfile>();
                c.AddProfile<SymbolProfile>();
                c.AddProfile<PositionProfile>();
            }).CreateMapper();
        }

        private async Task<IEnumerable<SymbolDto>> GetSymbolsAsync()
        {
            IEnumerable<SymbolDto> symbols;

            using (var con = new MySqlConnection(ConnectionString))
            {
                var query = @$"
                SELECT * 
                FROM Symbols";

                symbols = _mapper.Map<IEnumerable<SymbolDto>>(await con.QueryAsync<Symbol>(query));
            }

            return symbols;
        }
    }
}
