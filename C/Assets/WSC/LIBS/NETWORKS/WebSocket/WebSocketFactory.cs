namespace WSC
{
    public interface IWebSocketFactory
    {
        IWebSocket Create(string uri);
    }

    public class WebSocketFactory : IWebSocketFactory
    {
        public IWebSocket Create(string uri)
        {
            Log.Debug("WSC.WebSocket activated");
            return new WSC.WebSocket(uri);
        }
    }
}
