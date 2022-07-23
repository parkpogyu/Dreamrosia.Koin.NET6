using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Conveters;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "주문")]
    public class ExOrderPost : ExchangeClient<ExOrderPost.ExParameter>
    {
        public ExOrderPost() :
            base()
        {
            URL = string.Format("{0}/orders", ApiUrl);

            SetLimitedCount();
        }

        protected override void SetLimitedCount()
        {
            PerSecondLimted = 8;
            PerMinuteLimted = 200;
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = URL;

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<Order>> OrderPostAsync(object parameter)
        {
            Parameter = (ExParameter)parameter;

            try
            {
                var volume = Convert.ToDecimal(Parameter.volume);
                var price = Convert.ToDecimal(Parameter.price);

                if (string.IsNullOrEmpty(Parameter.market))
                {
                    return await Result<Order>.FailAsync("마케정보 누락");
                }

                if (Parameter.side == OrderSide.ask) // 매도
                {
                    // 시장가 매수
                    if (Parameter.ord_type == OrderType.price)
                    {
                        return await Result<Order>.FailAsync("주문방식 오류");
                    }

                    // 매도수량 필수
                    if (volume <= 0)
                    {
                        return await Result<Order>.FailAsync("매도수량 오류");
                    }

                    if (Parameter.ord_type == OrderType.limit)
                    {
                        // 지정가 매도금액 필수
                        if (price <= 0)
                        {
                            return await Result<Order>.FailAsync("매도가격 오류");
                        }
                    }
                    else if (Parameter.ord_type == OrderType.market)
                    {
                        Parameter.price = null;
                    }
                }
                else if (Parameter.side == OrderSide.bid) // 매수
                {
                    // 시장가 매도
                    if (Parameter.ord_type == OrderType.market)
                    {
                        return await Result<Order>.FailAsync("주문방식 오류");
                    }

                    if (Parameter.ord_type == OrderType.limit)
                    {
                        // 지정가 금액/수량 필수
                        if (price <= 0)
                        {
                            return await Result<Order>.FailAsync("매수가격 오류");
                        }
                        else if (volume <= 0)
                        {
                            return await Result<Order>.FailAsync("매수수량 오류");
                        }
                    }
                    else if (Parameter.ord_type == OrderType.price)
                    {
                        // 총 매수금액
                        if (price <= 0)
                        {
                            return await Result<Order>.FailAsync("주문총액 오류");
                        }

                        Parameter.volume = null;
                    }
                }
            }
            catch (Exception ex)
            {
                return await Result<Order>.FailAsync(ex.Message);
            }

            return await base.PostAsync<Order>(Parameter);
        }

        public class ExParameter : INotifyPropertyChanged, IWebApiParameter
        {
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// 마켓코드 (필수)
            /// </summary>
            public string market { get; set; }

            /// <summary>
            /// 주문종류 (필수)
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public OrderSide side { get; set; }

            /// <summary>
            /// 주문수량 (지정가, 시장가 매도 시 필수)
            /// </summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            [JsonConverter(typeof(NumericToStringConverter))]
            public decimal? volume { get; set; }

            /// <summary>
            /// 주문가격(필수)
            /// </summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            [JsonConverter(typeof(NumericToStringConverter))]
            public decimal? price { get; set; }

            /// <summary>
            /// 주문타입(필수)
            /// </summary>
            // 시장가 주문은 ord_type 필드를 price or market 으로 설정해야됩니다.
            // 매수 주문의 경우 ord_type을 price로 설정하고 volume을 null 혹은 제외해야됩니다.
            // 매도 주문의 경우 ord_type을 market로 설정하고 price을 null 혹은 제외해야됩니다.
            [JsonConverter(typeof(StringEnumConverter))]
            public OrderType ord_type { get; set; }

            /// <summary>
            /// 조회용 사용자 지정값 (선택)
            /// </summary>
            // identifier는 서비스에서 발급하는 uuid가 아닌 이용자가 직접 발급하는 키값으로, 주문을 조회하기 위해 할당하는 값입니다.
            // 해당 값은 사용자의 전체 주문 내 유일한 값을 전달해야하며, 비록 주문 요청시 오류가 발생하더라도 같은 값으로 다시 요청을 보낼 수 없습니다.
            // 주문의 성공 / 실패 여부와 관계없이 중복해서 들어온 identifier 값에서는 중복 오류가 발생하니, 매 요청시 새로운 값을 생성해주세요.
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string indendifier { get; set; }
        }
    }
}
