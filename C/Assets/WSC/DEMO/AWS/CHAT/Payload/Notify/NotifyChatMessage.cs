using System;
using WSC;

namespace DEMO.AWS.CHAT
{
    public class NotifyChatMessage : NotifyChat<NotifyChatMessage>
    {
        public string roomId;
        public string userId;
        public string message;
        public string name;
        public long timestamp;

        [RuntimeInitialize]
        private static void Initialize()
        {
            Register();
        }

        public override void OnQuery(Request request)
        {
            base.OnQuery(request);

            OnNotify?.Invoke(this);
        }

        public static event Action<NotifyChatMessage> OnNotify;
    }
}
