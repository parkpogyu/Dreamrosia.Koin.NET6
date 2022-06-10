using System;

namespace Dreamrosia.Koin.Application.Responses.Identity
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string Email { get; set; }
    }
}