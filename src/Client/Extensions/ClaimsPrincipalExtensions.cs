using Dreamrosia.Koin.Shared.Constants.Permission;
using System.Security.Claims;

namespace Dreamrosia.Koin.Client.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static string GetUserId(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        internal static string GetEmail(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
        internal static string GetNickName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        internal static string GetName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.KoreanName);
        internal static string GetProfileImage(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfielImange);
        internal static string GetExpire(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Expire);
    }
}