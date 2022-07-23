using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
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

        public async Task<IResult<IEnumerable<MarketTrade>>> GetMarketTradesAsync(object parameter)
        {
            Parameter = (QtParameter)parameter;

            return await base.GetAsync<IEnumerable<MarketTrade>>(parameter);
        }

        public class QtParameter : IWebApiParameter
        {
            /// <summary>
            /// 마켓코드
            /// </summary>
            public string market { get; set; }

            /// <summary>
            /// 최종시간, 비워서 요청시 가장 최근 데이터
            /// </summary>
            public DateTime? to { get; set; }

            /// <summary>
            /// 체결개수
            /// </summary>
            public int count { get; set; } = ClientConstants.MaxCount;

            /// <summary>
            /// 커서
            /// </summary>
            public string cursor { get; set; }

            /// <summary>
            /// 이전일, 비워서 요청 시 가장 최근 체결 날짜 반환. (범위: 1 ~ 7))
            /// </summary>
            public int? daysAgo { get; set; }
        }
    }
}
