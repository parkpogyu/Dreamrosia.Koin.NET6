using Dreamrosia.Koin.Application.Requests;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}