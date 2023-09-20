using WSC;

namespace DEMO
{
    public class NotifyWSC<T> : NotifyWS<T>
    {
        public NotifyWSC()
        {
            command = GetType().Name;
            host = WSCInfo.WSHost;
            cookies = WSCInfo.WSCookies;
        }
    }
}
