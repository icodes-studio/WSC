using WSC;

namespace AWS.CHAT
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

        public override void OnQuery(Request request)
        {
            base.OnQuery(request);

            Log.Debug($"roomId:{roomId}, userId:{userId}, name:{name}, message:{message}, timestamp:{timestamp}");
        }
    }
}
