using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "현재가 조회")]
    public class WsTicker : UPbitWebSocketClient
    {
        public WsTicker()
        {
            AutoReconnect = true;

            Parameter.Type.ReceivedType = WsParameter.ParameterType.ReceivedTypes.ticker;
            Parameter.Type.ReceivedMode = WsParameter.ParameterType.ReceivedModes.Realtime;
        }

        public event Action<WsResponse> OnMessageReceived;

        protected override void MessageReceived(byte[] message)
        {
            if (OnMessageReceived is null || message is null) { return; }

            OnMessageReceived(Deserialize<WsResponse>(message));
        }

        public class WsResponse : Ticker
        {
            public string type { get; set; }

            public string code
            {
                get => market;
                set { market = value; }
            }

            public OrderSide ask_bid { get; set; }

            public double acc_ask_volume { get; set; }

            public double acc_bid_volume { get; set; }

            public string trade_status { get; set; }

            public string market_state { get; set; }

            public string market_state_for_ios { get; set; }

            public bool is_trading_suspended { get; set; }

            public DelistingDate delisting_date { get; set; }

            public MarketAlert market_warning { get; set; }

            public WsParameter.ParameterType.ReceivedModes stream_type { get; set; }

            public class DelistingDate
            {
                public int year { get; set; }
                public int month { get; set; }
                public int day { get; set; }
            }
        }
    }
}
