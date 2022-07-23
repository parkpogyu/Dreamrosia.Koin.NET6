using Dreamrosia.Koin.WebApi.Infrastructure;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public interface IUPbitWebApiClient<TParameter> : IUPbitWebApiClient where TParameter : IWebApiParameter
    {
        TParameter Parameter { get; }
    }

    public interface IUPbitWebApiClient : IWebApiClient
    {
        int PerSecondLimted { get; }

        int PerMinuteLimted { get; }
    }
}
