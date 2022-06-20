using System;

namespace Dreamrosia.Koin.Application.Responses.Identity
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string NickName { get; set; }
        public string KoreanName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        #region Subscription
        /// <summary>
        /// 카카오 회원번호
        /// </summary>
        public string UserCode { get; set; }
        #endregion
    }
}