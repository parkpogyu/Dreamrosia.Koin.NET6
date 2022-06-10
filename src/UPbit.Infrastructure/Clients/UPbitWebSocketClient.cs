using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebSocket4Net;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public class UPbitWebSocketClient : IDisposable
    {
        public WsParameter Parameter { get; private set; } = new WsParameter();

        protected WebSocket client { get; private set; }

        protected bool AutoReconnect { get; set; } = false;

        private string uri => "wss://api.upbit.com/websocket/v1";

        private readonly JsonSerializer serializer = new JsonSerializer();

        public UPbitWebSocketClient()
        {
            client = new WebSocket(uri);

            client.EnableAutoSendPing = true;
            client.AutoSendPingInterval = 60;

            client.DataReceived += Client_DataReceived;

            client.Opened += Client_Opened;
            client.Closed += Client_Closed;
        }

        private void Client_Closed(object sender, EventArgs e)
        {
            if (!AutoReconnect) { return; }

            client.OpenAsync();
        }

        private void Client_Opened(object sender, EventArgs e)
        {
            Logger.log.Information(string.Format("{0}: WebSocket connection opened", this.GetType().Name));

            client.Send(Parameter.Message);
        }

        private void Client_DataReceived(object sender, DataReceivedEventArgs e)
        {
            MessageReceived(e.Data);
        }

        private void CheckWebSocketState()
        {
            if (client.State == WebSocketState.None ||
                client.State == WebSocketState.Closed)
            {
                client.OpenAsync();
            }
        }

        protected virtual void MessageReceived(byte[] message) { }

        protected T Deserialize<T>(byte[] bytes)
        {
            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream, Encoding.Default);

            try
            {
                return serializer.Deserialize<T>(new JsonTextReader(reader));
            }
            catch (Exception ex)
            {
                stream.Position = 0;

                Logger.log.Error(ex, reader.ReadToEnd());

                return default(T);
            }
        }

        public virtual void Send(IEnumerable<string> codes)
        {
            if (codes is null || !codes.Any()) { return; }

            Parameter.Type.codes.Clear();
            Parameter.Type.codes.AddRange(codes);

            CheckWebSocketState();

            if (!IsOpened) { return; }

            client.Send(Parameter.Message);
        }

        public void Close()
        {
            Logger.log.Information(string.Format("{0}: WebSocket connection closed", this.GetType().Name));

            client.Close();
        }

        public bool IsOpened => client.State == WebSocketState.Open ? true : false;

        public class WsParameter
        {
            ParameterTicket Ticket { get; set; } = new ParameterTicket();

            public ParameterType Type { get; private set; } = new ParameterType();

            public ParameterFormat Format { get; private set; } = new ParameterFormat();

            public string Message
            {
                get
                {
                    List<string> json = new List<string>();

                    json.Add(JsonConvert.SerializeObject(Ticket, Formatting.None));
                    json.Add(JsonConvert.SerializeObject(Type, Formatting.None));
                    json.Add(JsonConvert.SerializeObject(Format, Formatting.None));

                    return string.Format("[{0}]", string.Join(",", json));
                }
            }

            class ParameterTicket
            {
                public string ticket => Guid.NewGuid().ToString();
            }

            public class ParameterType
            {
                [JsonIgnore]
                public ReceivedTypes ReceivedType { get; set; } = ReceivedTypes.ticker;

                public string type => ReceivedType.ToString();

                //codes 수신할 시세 종목 정보.
                //주의 : codes 필드에 명시되는 종목들은 대문자로 요청해야 합니다.
                public List<string> codes { get; private set; } = new List<string>();

                [JsonIgnore]
                public ReceivedModes ReceivedMode { get; set; } = ReceivedModes.Realtime;

                //isOnlySnapshot 시세 스냅샷만 제공  
                public bool isOnlySnapshot => ReceivedMode == ReceivedModes.Snapshot ? true : false;

                //isOnlyRealtime 실시간 시세만 제공
                public bool isOnlyRealtime => ReceivedMode == ReceivedModes.Realtime ? true : false;

                public enum ReceivedModes
                {
                    Both,
                    Snapshot,
                    Realtime,
                }

                public enum ReceivedTypes
                {
                    ticker,
                    trade,
                    orderbook,
                }
            }

            public class ParameterFormat
            {
                // SIMPLE -> 간소화된 필드명
                // DEFAULT
                public string format => IsDefault ? "DEFAULT" : "SIMPLE";

                [JsonIgnore]
                public bool IsDefault { get; set; } = true;
            }
        }

        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                    client.Close();
                    client.Dispose();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~WebSocketClient()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
