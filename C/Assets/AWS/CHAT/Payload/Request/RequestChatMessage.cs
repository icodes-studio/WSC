using WSC;

namespace AWS.CHAT
{
    public class RequestChatMessage : RequestChat
    {
        public string text;
        public string userId;
        public string name;

        public RequestChatMessage()
        {
            method = NetworkW3.PUT;
            userId = ChatInfo.UID;
            name = ChatInfo.NAME;
        }
    }
}