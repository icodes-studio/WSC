using WSC;

namespace AWS.CHAT
{
    public class NotifyChat<T> : NotifyWS<T>
    {
        public NotifyChat()
        {
            host = ChatInfo.WSHost;
        }
    }
}
