using System.Collections.Generic;

namespace WSC
{
    public class NotifyWS<T> : Notify
    {
        protected static void Register()
        {
            NetworkWSClient.i.RegisterNotify(typeof(T));
        }

        protected static void Register(string command)
        {
            NetworkWSClient.i.RegisterNotify(typeof(T), command);
        }

        protected static void Register(string host, string command)
        {
            NetworkWSClient.i.RegisterNotify(typeof(T), host, command);
        }

        protected static void Register(string host, Dictionary<string, string> cookies, string command)
        {
            NetworkWSClient.i.RegisterNotify(typeof(T), host, cookies, command);
        }
    }
}
