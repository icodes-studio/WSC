using System;

namespace WSC
{
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

    public interface IWebSocketFactory
    {
        IWebSocket Create(string uri);
    }

    public sealed class WebSocketFactory : IWebSocketFactory
    {
        public IWebSocket Create(string uri)
        {
            Log.Debug($"WSC.WebSocket(WebSocketSharp) activated with {uri}");
            return new WSC.WebSocket(uri);
        }
    }
}
