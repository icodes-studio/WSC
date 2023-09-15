namespace WSC.DEMO
{
    public class WebProtocolFactory : IWebProtocolFactory
    {
        public IWebRequest CreateWebRequest() =>
            new global::WSC.DEMO.WebRequest();

        public IWebSocket CreateWebSocket(string uri) =>
            new global::WSC.DEMO.WebSocket(uri);
    }
}
