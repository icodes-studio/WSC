namespace WSC.UNITY
{
    public class WebProtocolFactory : IWebProtocolFactory
    {
        public IWebRequest CreateWebRequest() =>
            new global::WSC.UNITY.WebRequest();

        public IWebSocket CreateWebSocket(string uri) =>
            new global::WSC.UNITY.WebSocket(uri);
    }
}
