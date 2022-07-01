using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "캔들 조회")]
    public class QtCandle : QuotaionClient<QtCandle.QtParameter>
    {
        public QtCandle()
        {
            URL = string.Format("{0}/candles", ApiUrl);
        }

        protected override List<KeyValuePair<string, object>> QueryParameters()
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("market", Parameter.market),
                new KeyValuePair<string, object>("count", Parameter.count),
            };

            if (Parameter.to != null)
            {
                parameters.Add(new KeyValuePair<string, object>("to", ((DateTime)Parameter.to).ToString("yyyy-MM-dd HH:mm:ss")));
            }

            return parameters;
        }

        protected override void SetHeadersAndURI()
        {
            if (Parameter.TimeFrame == TimeFrames.Minute)
            {
                URI = string.Format("{0}/minutes/{1}?{2}", URL, (int)Parameter.unit, QueryString(QueryParameters()));
            }
            else if (Parameter.TimeFrame == TimeFrames.Day)
            {
                var parameters = QueryParameters();

                parameters.Add(new KeyValuePair<string, object>("convertingPriceUnit", Parameter.convertingPriceUnit));

                URI = string.Format("{0}/days?{1}", URL, QueryString(parameters));
            }
            else if (Parameter.TimeFrame == TimeFrames.Week)
            {
                URI = string.Format("{0}/weeks?{1}", URL, QueryString(QueryParameters()));
            }
            else if (Parameter.TimeFrame == TimeFrames.Month ||
                     Parameter.TimeFrame == TimeFrames.Year)
            {
                URI = string.Format("{0}/months?{1}", URL, QueryString(QueryParameters()));
            }
        }

        public async Task<IResult<IEnumerable<Candle>>> GetCandlesAsync(object parameter)
        {
            Parameter = (QtParameter)parameter;

            return await base.GetAsync<IEnumerable<Candle>>(parameter);
        }

        public class QtParameter : IWebApiParameter
        {
            /// <summary>
            /// 캔들주기
            /// </summary>
            public TimeFrames TimeFrame { get; set; } = TimeFrames.Day;

            /// <summary>
            /// 마켓코드
            /// </summary>
            public string market { get; set; }

            /// <summary>
            /// 최종시간
            /// </summary>
            public DateTime? to { get; set; }

            /// <summary>
            /// 캔들개수
            /// </summary>
            public int count { get; set; } = ClientConstants.MaxCount;

            #region Minutes
            /// <summary>
            /// 분단위
            /// </summary>
            public MinutesUnit unit { get; set; } = MinutesUnit._1;
            #endregion

            #region Day
            /// <summary>
            /// 종가 환산 화폐 단위
            /// </summary>
            public string convertingPriceUnit { get; set; } = "KRW";
            #endregion
        }
    }
}
