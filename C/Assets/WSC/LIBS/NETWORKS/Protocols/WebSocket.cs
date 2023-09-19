using System;
using System.Web;
using System.Collections.Specialized;
using System.Threading.Tasks;
using WebSocketSharp.Net;

namespace WSC
{
    public sealed class WebSocket : IWebSocket
    {
        private WebSocketSharp.WebSocket socket = null;

        public event Action<ushort> OnClose = delegate { };
        public event Action<string> OnError = delegate { };
        public event Action<string> OnMessage = delegate { };
        public event Action OnOpen = delegate { };

        public WebSocket(string uri)
        {
            socket = new WebSocketSharp.WebSocket(uri);
            socket.OnOpen += (s, e) => OnOpen();
            socket.OnMessage += (s, e) => OnMessage(e?.Data);
            socket.OnError += (s, e) => OnError(e?.Message);
            socket.OnClose += (s, e) => OnClose(e?.Code ?? 0);

            if (!string.IsNullOrEmpty(Uri.Query))
            {
                NameValueCollection cookies = HttpUtility.ParseQueryString(Uri.Query);
                foreach (var key in cookies.AllKeys)
                    socket.SetCookie(new Cookie(key, cookies[key]));
            }

            if (socket.IsSecure == true)
                socket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
#if DEBUG
            socket.Log.Level = WebSocketSharp.LogLevel.Trace;
#endif
        }

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

        public Uri Uri => socket?.Url;

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
    }
}
