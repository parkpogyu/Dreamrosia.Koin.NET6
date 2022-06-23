using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class UserBriefDto : UserCardDto
    {
        public DateTime CreatedOn { get; set; }

        #region Subscription
        /// <summary>
        /// 카카오 회원번호
        /// </summary>
        public string UserCode { get; set; }
        #endregion
    }
}
