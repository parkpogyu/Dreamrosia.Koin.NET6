using Dreamrosia.Koin.UPbit.Infrastructure.Models;


namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public class ExchangeClient<TParameter> : UPbitWebApiClient<TParameter>, IExchangeClient where TParameter : IWebApiParameter
    {
        // Exchange API
        // =======================================================
        // [주문 요청]
        // 초당 8회, 분당 200회
        // -------------------------------------------------------
        // [주문 요청 외 API]
        // 초당 30회, 분당 900회

        public ExchangeClient()
        {
            SetLimitedCount();
        }

        protected virtual void SetLimitedCount()
        {
            PerSecondLimted = 30;
            PerMinuteLimted = 900;
        }

        public static int MaxResponse => 100;
    }

    public interface IExchangeClient { }

    public class ExchangeClientKeys : UPbitKey
    {
        public static string AccessKey { get; private set; }

        public static string SecretKey { get; private set; }

        public static void SetAuthenticationKey(UPbitKey key)
        {
            AccessKey = key.access_key;
            SecretKey = key.secret_key;
        }
    }
}
