using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "호가정보 조회")]
    public class QtOrderBook : QuotaionClient<QtOrderBook.QtParameter>
    {
        public QtOrderBook()
        {
            URL = string.Format("{0}/orderbook", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?markets={1}", URL, string.Join(",", Parameter.markets));
        }

        public class QtParameter : IWebApiParameter
        {
            [Display(Name = "마켓코드")]
            public List<string> markets { get; private set; } = new List<string>();
        }
    }
}
