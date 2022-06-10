using Dreamrosia.Koin.Application.Responses.Audit;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Audit
{
    public interface IAuditManager : IManager
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync();

        Task<IResult<string>> DownloadFileAsync(string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false);
    }
}