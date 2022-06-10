using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.Requests.Identity
{
    public class UpdateProfileRequest
    {
        [Required]
        public string NickName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}