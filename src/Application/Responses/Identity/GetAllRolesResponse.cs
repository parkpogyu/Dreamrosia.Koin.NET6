using System.Collections.Generic;

namespace Dreamrosia.Koin.Application.Responses.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}