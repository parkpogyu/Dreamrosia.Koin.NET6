namespace Dreamrosia.Koin.Application.DTO
{
    public class SubscriptionDto : UserSummaryDto
    {
        public MembershipDto Membership { get; set; }

        public UserDto Recommender { get; set; }
    }
}