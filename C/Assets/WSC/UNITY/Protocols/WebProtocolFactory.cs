using System.Collections.Generic;

namespace WSC.UNITY
{
    public class WebProtocolFactory : IWebProtocolFactory
    {
        public IWebRequest CreateWebRequest() =>
            new global::WSC.UNITY.WebRequest();

        public IWebSocket CreateWebSocket(string uri, Dictionary<string, string> cookies) =>
            new global::WSC.UNITY.WebSocket(uri, cookies);
    }
}
