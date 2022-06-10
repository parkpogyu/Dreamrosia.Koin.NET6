using Dreamrosia.Koin.Application.Specifications.Base;
using Dreamrosia.Koin.Infrastructure.Models.Identity;

namespace Dreamrosia.Koin.Infrastructure.Specifications
{
    public class UserFilterSpecification : HeroSpecification<BlazorHeroUser>
    {
        public UserFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.NickName.Contains(searchString) ||
                                p.Email.Contains(searchString) ||
                                p.PhoneNumber.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}