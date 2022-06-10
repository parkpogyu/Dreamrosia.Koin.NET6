using Dreamrosia.Koin.Application.Interfaces.Common;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
    }
}