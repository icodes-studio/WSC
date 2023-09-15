using WSC;

namespace AWS.CHAT
{
    public class RequestChatHistory : RequestChat
    {
        public RequestChatHistory()
        {
            method = WebRequest.GET;
        }
    }
}