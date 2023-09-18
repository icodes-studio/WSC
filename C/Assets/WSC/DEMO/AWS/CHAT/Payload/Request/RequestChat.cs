using WSC;

namespace DEMO.AWS.CHAT
{
    public class RequestChat : RequestW3
    {
        public string roomId;

        public RequestChat()
        {
            roomId = ChatInfo.RID;
            host = ChatInfo.W3Host;
        }
    }
}