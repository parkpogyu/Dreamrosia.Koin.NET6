using Dreamrosia.Koin.Application.Interfaces.Common;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> KakaoLoginAsync();

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}