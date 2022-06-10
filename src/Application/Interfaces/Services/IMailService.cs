using Dreamrosia.Koin.Application.Requests.Mail;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}