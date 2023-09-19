using System;
using System.Collections.Generic;
using System.Text;

namespace WSC.UNITY
{
    public class WebSocket : IWebSocket
    {
        private NativeWebSocket.WebSocket socket = null;

        public event Action<ushort> OnClose = delegate { };
        public event Action<string> OnError = delegate { };
        public event Action<string> OnMessage = delegate { };
        public event Action OnOpen = delegate { };

        public WebSocket(string uri)
        {
            Uri = new Uri(uri);
            Dictionary<string, string> header = null;
            if (!string.IsNullOrEmpty(Uri.Query))
            {
                header = new Dictionary<string, string>();
                header["Cookie"] = Uri.Query.TrimStart('?').Replace("&", ";");
            }

            socket = new NativeWebSocket.WebSocket(uri, header);
            socket.OnOpen += () => OnOpen();
            socket.OnMessage += (message) => OnMessage(Encoding.UTF8.GetString(message));
            socket.OnError += (message) => OnError(message);
            socket.OnClose += (code) => OnClose((ushort)code);
        }

        public Uri Uri
        {
            get;
            private set;
        }

        public void Connect()
        {
            socket?.Connect();
        }

        public void Send(string message, Action<bool> result)
        {
            socket?.SendText(message).ContinueWith((task) => result?.Invoke(!task.IsFaulted));
        }

        public void Close()
        {
            socket?.Close();
        }

        public void Dispatch()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            socket?.DispatchMessageQueue();
#endif
        }
    }
}
