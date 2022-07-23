using Dreamrosia.Koin.WebApi.Infrastructure;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public class QuotaionClient<TParameter> : UPbitWebApiClient<TParameter>, IQuotaionClient where TParameter : IWebApiParameter
    {
        // Quotation API
        // REST API 요청 수 제한
        // ------------------------------------------------------
        // 분당 600회, 초당 10회(종목, 캔들, 체결, 티커, 호가별)
        public QuotaionClient()
        {
            PerSecondLimted = 10;
            PerMinuteLimted = 600;
        }
    }

    public interface IQuotaionClient
    {
    }
}
