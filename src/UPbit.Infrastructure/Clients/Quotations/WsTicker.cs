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

            // 타입     필드                    설명
            //-------------------------------------------------------------------------------------
            // String            type ty        타입
            //                                  ticker : 현재가
            // String code                      마켓 코드
            //                                  (ex.KRW-BTC) 
            // String ask_bid ab                매수/매도 구분
            //                                  ASK: 매도
            //                                  BID : 매수
            // Double acc_ask_volume            누적 매도량 
            // Double acc_bid_volume            누적 매수량  
            // Double highest_52_week_price   	52주 최고가 
            // String highest_52_week_date    	52주 최고가 달성일 
            // String lowest_52_week_price    	52주 최저가
            // String lowest_52_week_date 	    52주 최저가 달성일 
            // String market_state              거래상태
            //                                  PREVIEW : 입금지원
            //                                  ACTIVE : 거래지원가능
            //                                  DELISTED : 거래지원종료
            // Boolean is_trading_suspended     거래 정지 여부    
            // Date    delisting_date           상장폐지일   
            // String  market_warning           유의 종목 여부    
            //                                  NONE : 해당없음
            //                                  CAUTION : 투자유의
            // Long    timestamp                타임스탬프(milliseconds)    
            // String  stream_type              스트림 타입 
            //                                  SNAPSHOT : 스냅샷
            //                                  REALTIME : 실시간
            //-------------------------------------------------------------------------------------
            public string type { get; set; }

            public string code
            {
                get => market;
                set { market = value; }
            }

            public OrderSide ask_bid { get; set; }
            public decimal acc_ask_volume { get; set; }
            public decimal acc_bid_volume { get; set; }
            public string market_state { get; set; }
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
