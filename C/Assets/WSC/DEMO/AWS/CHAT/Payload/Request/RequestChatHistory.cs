using WSC;

namespace DEMO.AWS.CHAT
{
    public class RequestChatHistory : RequestChat
    {
        public RequestChatHistory()
        {
            method = WebRequest.GET;
        }
    }
}