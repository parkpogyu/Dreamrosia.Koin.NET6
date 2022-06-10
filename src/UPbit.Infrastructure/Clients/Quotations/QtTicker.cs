using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "현재가 조회")]
    public class QtTicker : QuotaionClient<QtTicker.QtParameter>
    {
        public QtTicker()
        {
            URL = string.Format("{0}/ticker", ApiUrl);
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
