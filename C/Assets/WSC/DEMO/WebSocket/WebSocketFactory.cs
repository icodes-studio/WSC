namespace WSC.DEMO
{
    public class WebSocketFactory : IWebSocketFactory
    {
        public IWebSocket Create(string uri)
        {
#if WEBSOCKETSHARP
            // https://github.com/sta/websocket-sharp
            Log.Debug($"WSC.WebSocket(WebSocketSharp) activated with {uri}");
            return new global::WSC.WebSocket(uri);
#else
            // https://github.com/endel/NativeWebSocket
            Log.Debug($"WSC.DEMO.WebSocket(NativeWebSocket) activated with {uri}");
            return new global::WSC.DEMO.WebSocket(uri);
#endif
        }
    }
}
