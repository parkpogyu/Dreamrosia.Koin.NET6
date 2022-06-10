using System.Collections.Generic;

namespace Dreamrosia.Koin.Application.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}