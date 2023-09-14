using WSC;

namespace AWS.CHAT
{
    public class RequestChatHistory : RequestChat
    {
        public RequestChatHistory()
        {
            method = WebRequestTypes.GET;
        }
    }
}