using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.Requests.Identity
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}