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
    }
}
