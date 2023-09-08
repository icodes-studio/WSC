using System.Collections.Generic;

namespace AWS.CHAT
{
    public class AnswerChatHistory : AnswerChat
    {
        public class Message
        {
            public string roomId;
            public string userId;
            public string message;
            public string name;
            public long timestamp;
        }

        public List<Message> messages = new ();
    }
}
