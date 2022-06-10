using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure
{
    [Display(Name = "체결 목록 조회")]
    public class QtMarketTrade : QuotaionClient<QtMarketTrade.QtParameter>
    {
        public QtMarketTrade()
        {
            URL = string.Format("{0}/trades/ticks", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?{1}", URL, QueryString(Parameter));
        }

        public class QtParameter : IWebApiParameter
        {
            [Display(Name = "마켓코드")]
            public string market { get; set; }

            // 비워서 요청시 가장 최근 데이터
            [Display(Name = "최종시간")]
            public DateTime? to { get; set; }

            [Display(Name = "체결개수")]
            public int count { get; set; } = ClientConstants.MaxCount;

            [Display(Name = "커서")]
            public string cursor { get; set; }

            // 비워서 요청 시 가장 최근 체결 날짜 반환. (범위: 1 ~ 7))
            [Display(Name = "이전일")]
            public int? daysAgo { get; set; }
        }

    }
}
