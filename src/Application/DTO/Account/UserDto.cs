namespace Dreamrosia.Koin.Application.DTO
{
    public class UserDto : UserBriefDto
    {
        public string Id { get; set; }

        public string KoreanName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
