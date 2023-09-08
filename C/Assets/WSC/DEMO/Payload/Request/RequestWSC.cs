using System.Collections.Generic;

namespace WSC.DEMO
{
    public class RequestWSC :
#if W3C
        RequestW3
#else
        RequestWS
#endif
    {
        public string version;
        public string platform;
        public string locale;

        public RequestWSC()
        {
            command = GetType().Name;
            version = WSCInfo.Version;
            platform = WSCInfo.Platform;
            locale = WSCInfo.Locale;
#if W3C
            host = WSCInfo.W3Host;
            headers = new Dictionary<string, string>
            {
                {"Authorization", string.Format("Bearer {0}", WSCInfo.AccessToken)},
                {"Content-Type", "application/json"}
            };
#else
            host = WSCInfo.WSHost;
#endif
        }
    }
}