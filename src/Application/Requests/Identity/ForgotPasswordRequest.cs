using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}