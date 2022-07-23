using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "주문 목록 조회")]
    public class ExOrders : ExchangeClient<ExOrders.ExParameter>
    {

        private readonly ExOrder ExOrder = new ExOrder();

        public ExOrders()
        {
            URL = string.Format("{0}/orders", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public Task<IResult<IEnumerable<Order>>> GetCompletedOrdersAsync(DateTime? to = null)
        {
            var states = new List<OrderState>()
            {
                 OrderState.done,
                 OrderState.cancel,
            };

            return GetOrdersAsync(states, to);
        }

        public Task<IResult<IEnumerable<Order>>> GetWaitingOrdersAsync()
        {
            var states = new List<OrderState>()
            {
                 OrderState.wait,
                 OrderState.watch,
            };

            return GetOrdersAsync(states, null);
        }

        private async Task<IResult<IEnumerable<Order>>> GetOrdersAsync(IEnumerable<OrderState> states, DateTime? to)
        {
            Parameter = new ExParameter();

            Parameter.page = 1;
            Parameter.states.Clear();
            Parameter.states.AddRange(states);

            var orders = new List<Order>();

            var limit = Convert.ToDateTime(to);

            IResult<IEnumerable<Order>> response = null;

            while (true)
            {
                response = await GetAsync<IEnumerable<Order>>(Parameter);

                if (!response.Succeeded) { break; }
                if (!response.Data.Any()) { break; }

                if (response.Data.Any(f => f.created_at <= limit))
                {
                    orders.AddRange(response.Data.Where(f => f.created_at >= limit));

                    break;
                }
                else
                {
                    orders.AddRange(response.Data);
                }

                if (response.Data.Count() <= MaxResponse) { Parameter.page++; }
            }

            if (response.Succeeded)
            {
                var items = orders.Where(f => !(f.state == OrderState.cancel && f.trades_count == 0));

                return await Result<IEnumerable<Order>>.SuccessAsync(items);
            }
            else
            {
                return response;
            }
        }

        public async Task GetTradesAsync(IEnumerable<Order> orders)
        {
            ExOrder.ExParameter parameter = new ExOrder.ExParameter();

            List<Order> uncompleted = new List<Order>();

            foreach (var order in orders)
            {
                parameter.uuid = order.uuid;

                var response = await ExOrder.GetOrderAsync(parameter);

                if (response.Succeeded)
                {
                    var trades = response.Data.Trades;

                    if (trades.Any())
                    {
                        var funds = trades.Sum(f => f.funds);
                        var volumes = trades.Sum(f => f.volume);

                        if (volumes > 0)
                        {
                            order.avg_price = Math.Round((double)(funds / volumes), 4);
                        }

                        order.exec_amount = Math.Round((double)funds, 4);
                    }
                }
                else
                {
                    uncompleted.Add(order);
                }
            }

            if (uncompleted.Any())
            {
                await GetTradesAsync(uncompleted);
            }
        }

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 마켓코드
            /// </summary>
            public string market { get; set; }

            /// <summary>
            /// 주문상태
            /// </summary>
            public OrderState? state { get; set; }

            /// <summary>
            /// 주문상태목록
            /// </summary>
            public List<OrderState> states { get; private set; } = new List<OrderState>();

            /// <summary>
            /// 주문번호
            /// </summary>
            public List<string> uuids { get; private set; } = new List<string>();

            /// <summary>
            /// 조회번호
            /// </summary>
            public List<string> identifiers { get; private set; } = new List<string>();

            /// <summary>
            /// 페이지
            /// </summary>
            public int? page { get; set; } = 1;

            /// <summary>
            /// 요청개수
            /// </summary>
            public int? limit { get; set; } = MaxResponse;

            /// <summary>
            /// 정렬방식
            /// </summary>
            public OrderBy? order_by { get; set; } = OrderBy.desc;
        }
    }
}
