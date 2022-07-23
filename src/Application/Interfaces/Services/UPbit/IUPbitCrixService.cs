using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IUPbitCrixService
    {
        Task<IResult> GetCrixesAsync();
    }
}
