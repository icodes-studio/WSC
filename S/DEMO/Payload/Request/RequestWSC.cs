using System;
using System.Collections.Generic;

namespace WSC.DEMO
{
    public class RequestWSC<T> :
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
                {"Authorization", string.Format("Bearer {0}", WSCInfo.AccessToken)}
            };
#else
            host = WSCInfo.WSHost;
#endif
        }

        protected static void Register()
        {
#if W3C
            Program.i.RegisterHandler((Activator.CreateInstance(typeof(T)) as RequestW3).command, typeof(T));
#else
            Program.i.RegisterHandler((Activator.CreateInstance(typeof(T)) as RequestWS).command, typeof(T));
#endif
        }
    }
}
