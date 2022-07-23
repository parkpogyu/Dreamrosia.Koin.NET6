
namespace Dreamrosia.Koin.CoinMarketCap.Infrastructure.Clients
{
    public interface ICoinMarketCapWebApiClient<IWebApiParameter>
    {
        IWebApiParameter Parameter { get; }
    }
}
