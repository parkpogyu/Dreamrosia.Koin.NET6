using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IAssetService
    {
        Task<IResult<AssetReportDto>> GetAssetsAsync(string userId);
    }
}