using System.Collections.Generic;
using WSC;

namespace AWS.CHAT
{
    public class AnswerChatMessage : AnswerChat
    {
        public class UserInfo
        {
            public string userId;
            public string roomId;
            public string connectionId;
            public long timestamp;
        }

        public List<UserInfo> users = new ();

        public override void OnQuery(Request request)
        {
            base.OnQuery(request);

            foreach (var user in users)
                Log.Debug($"userId:{user.userId}, roomId:{user.roomId}, connectionId:{user.connectionId}, timestamp:{user.timestamp}");
        }
    }
}
