namespace WSC
{
    public class NotifyWSC<T> : NotifyWS<T>
    {
        public NotifyWSC()
        {
            command = GetType().Name;
            host = "ws://" + WSCInfo.Host;
        }
    }
}
