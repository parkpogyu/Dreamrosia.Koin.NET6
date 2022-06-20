using Dreamrosia.Koin.Application.Responses.Identity;

namespace Dreamrosia.Koin.Application.DTO
{
    public class UserDetailDto : UserSummaryDto
    {

        public MembershipDto Membership { get; set; }
        public UserResponse Recommender { get; set; }
    }
}