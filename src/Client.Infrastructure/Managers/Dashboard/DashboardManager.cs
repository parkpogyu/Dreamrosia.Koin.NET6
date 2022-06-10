using Dreamrosia.Koin.Application.Features.Dashboards;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Dashboard
{
    public class DashboardManager : IDashboardManager
    {
        private readonly HttpClient _httpClient;

        public DashboardManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<DashboardDataResponse>> GetDataAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DashboardEndpoints.GetData);
            var data = await response.ToResult<DashboardDataResponse>();
            return data;
        }
    }
}