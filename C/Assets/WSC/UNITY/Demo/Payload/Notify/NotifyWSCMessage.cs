namespace WSC.UNITY.DEMO
{
    public class NotifyWSCMessage : NotifyWSC<NotifyWSCMessage>
    {
        public long type;
        public string message;

        [RuntimeInitialize]
        internal static void Initialize()
        {
            Register();
        }

        internal override void OnQuery(Request request)
        {
            base.OnQuery(request);

            Log.Debug($"type:{type}, message:{message}");
        }
    }
}
