using WSC;

namespace DEMO.AWS.CHAT
{
    public class NotifyChat<T> : NotifyWS<T> where T : class
    {
        public NotifyChat()
        {
            host = ChatInfo.WSHost;
        }
    }
}
