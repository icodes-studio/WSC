namespace WSC.DEMO
{
    public class WebSocketFactory : IWebSocketFactory
    {
        public IWebSocket Create(string uri)
        {
#if WEBSOCKETSHARP
            Log.Debug("WSC.WebSocket activated");
            return new global::WSC.WebSocket(uri);
#else
            Log.Debug("WSC.DEMO.WebSocket activated");
            return new global::WSC.DEMO.WebSocket(uri);
#endif
        }
    }
}
