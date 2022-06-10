namespace Dreamrosia.Koin.Application.DTO
{
    public class UserDetailDto : UserSummaryDto
    {

        public MembershipDto Membership { get; set; }
        public UserSummaryDto Recommender { get; set; }
    }
}