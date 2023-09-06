using System;
using System.Threading.Tasks;

namespace WSC
{
    public enum CloseStatusCode : ushort
    {
        NotSet = 0,
        Normal = 1000,
        Away = 1001,
        ProtocolError = 1002,
        UnsupportedData = 1003,
        Undefined = 1004,
        NoStatus = 1005,
        Abnormal = 1006,
        InvalidData = 1007,
        PolicyViolation = 1008,
        TooBig = 1009,
        MandatoryExtension = 1010,
        ServerError = 1011,
        TlsHandshakeFailure = 1015
    }

    public interface IWebSocket
    {
        event Action<ushort> OnClose;
        event Action<string> OnError;
        event Action<string> OnMessage;
        event Action OnOpen;

        Uri Uri { get; }
        void Connect();
        void Send(string message, Action<bool> result);
        void Close();
        void Dispatch();
    }

    public class WebSocket : IWebSocket
    {
        private WebSocketSharp.WebSocket socket = null;

        public event Action<ushort> OnClose = delegate { };
        public event Action<string> OnError = delegate { };
        public event Action<string> OnMessage = delegate { };
        public event Action OnOpen = delegate { };

        public WebSocket(string uri)
        {
            socket = new WebSocketSharp.WebSocket(uri);
            socket.OnOpen += (_, _) => OnOpen();
            socket.OnMessage += (_, arg) => OnMessage(arg?.Data);
            socket.OnError += (_, arg) => OnError(arg.Message);
            socket.OnClose += (_, arg) => OnClose(arg.Code);

            if (socket.IsSecure == true)
                socket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
#if DEBUG
            socket.Log.Level = WebSocketSharp.LogLevel.Trace;
#endif
        }

        public Uri Uri => socket?.Url;

        public void Connect()
        {
#if NETCOREAPP
            Task.Run(() => socket?.Connect());
#else
            socket?.ConnectAsync();
#endif
        }

        public void Send(string message, Action<bool> result)
        {
#if NETCOREAPP
            Task.Run(() => socket?.Send(message)).ContinueWith((task) => result?.Invoke(task.IsCompletedSuccessfully));
#else
            socket?.SendAsync(message, result);
#endif
        }

        public void Close()
        {
            socket?.Close((ushort)CloseStatusCode.Normal);
        }

        public void Dispatch()
        {
            // N/A
        }
    }
}
