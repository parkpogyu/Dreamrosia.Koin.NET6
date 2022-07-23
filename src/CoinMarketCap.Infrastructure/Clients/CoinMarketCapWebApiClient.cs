using Dreamrosia.Koin.WebApi.Infrastructure;

namespace Dreamrosia.Koin.CoinMarketCap.Infrastructure.Clients
{
    public class CoinMarketCapWebApiClient<IWebApiParameter> : WebApiClient
    {
        protected string ApiUrl => "https://api.coinmarketcap.com/data-api/v3/cryptocurrency";

        public IWebApiParameter Parameter { get; protected set; }

        public CoinMarketCapWebApiClient()
            : base()
        {
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
        }
    }
}
