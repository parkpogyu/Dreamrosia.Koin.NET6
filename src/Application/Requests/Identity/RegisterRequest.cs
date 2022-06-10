using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.Requests.Identity
{
    public class RegisterRequest
    {
        [Required]
        public string NickName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }

        public bool ActivateUser { get; set; } = false;

        public bool AutoConfirmEmail { get; set; } = false;
    }
}