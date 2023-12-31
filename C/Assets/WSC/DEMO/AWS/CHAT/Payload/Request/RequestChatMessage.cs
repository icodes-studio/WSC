using WSC;

namespace DEMO.AWS.CHAT
{
    public class RequestChatMessage : RequestChat
    {
        public string text;
        public string userId;
        public string name;

        public RequestChatMessage()
        {
            method = WebRequest.PUT;
            userId = ChatInfo.UID;
            name = ChatInfo.NAME;
        }
    }
}