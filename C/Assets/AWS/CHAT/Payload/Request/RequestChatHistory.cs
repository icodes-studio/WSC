using WSC;

namespace AWS.CHAT
{
    public class RequestChatHistory : RequestChat
    {
        public RequestChatHistory()
        {
            method = NetworkW3.GET;
        }
    }
}