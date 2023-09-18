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
        internal static void Initialize()
        {
            Register();
        }

        internal override void OnQuery(Request request)
        {
            base.OnQuery(request);

            OnNotify?.Invoke(this);
        }

        public static event Action<NotifyChatMessage> OnNotify;
    }
}
