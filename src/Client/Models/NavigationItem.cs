using Dreamrosia.Koin.Application.DTO;

namespace Dreamrosia.Koin.Client.Models
{
    public class NavigationItem : UserCardDto
    {
        public string UserId { get; set; }

        public string URL { get; set; }
    }
}
