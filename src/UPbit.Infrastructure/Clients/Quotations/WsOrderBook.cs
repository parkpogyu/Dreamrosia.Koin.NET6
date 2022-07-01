using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "호가정보 조회")]
    public class WsOrderBook : UPbitWebSocketClient
    {
        private static readonly Lazy<WsOrderBook> instance = new Lazy<WsOrderBook>(() => new WsOrderBook());

        public static WsOrderBook Instance => instance.Value;

        public List<OrderBook.Unit> Units { get; private set; } = new List<OrderBook.Unit>();

        private WsOrderBook()
        {
            Parameter.Type.ReceivedType = WsParameter.ParameterType.ReceivedTypes.orderbook;
            Parameter.Type.ReceivedMode = WsParameter.ParameterType.ReceivedModes.Realtime;
        }

        public WsResponse Response { get; private set; }

        protected override void MessageReceived(byte[] message)
        {
            try
            {
                Response = Deserialize<WsResponse>(message);

                if (Response is null) { return; }

                List<OrderBook.Unit> units = new List<OrderBook.Unit>();

                var book = Response;

                foreach (var unit in book.orderbook_units.OrderByDescending(f => f.ask_price))
                {
                    units.Add(new OrderBook.Unit()
                    {
                        price = unit.ask_price,
                        ask_size = unit.ask_size,
                    });
                }

                foreach (var unit in book.orderbook_units)
                {
                    units.Add(new OrderBook.Unit()
                    {
                        price = unit.bid_price,
                        bid_size = unit.bid_size,
                    });
                }

                if (Units.Any())
                {
                    int count = units.Count;

                    for (int i = 0; i < count; i++)
                    {
                        Units[i].ask_size = units[i].ask_size;
                        Units[i].bid_size = units[i].bid_size;
                        Units[i].price = units[i].price;
                    }
                }
                else
                {
                    Units.AddRange(units);
                }
            }
            catch (Exception)
            {
            }
        }

        public class WsResponse : OrderBook
        {
            public string type { get; set; }

            public string code
            {
                get => market;
                set { market = value; }
            }
        }
    }
}
