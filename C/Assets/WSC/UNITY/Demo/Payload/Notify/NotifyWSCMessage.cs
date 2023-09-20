namespace WSC.UNITY.DEMO
{
    public class NotifyWSCMessage : NotifyWSC<NotifyWSCMessage>
    {
        public long type;
        public string message;

        [RuntimeInitialize]
        private static void Initialize()
        {
            Register();
        }

        public override void OnQuery(Request request)
        {
            base.OnQuery(request);

            Log.Debug($"type:{type}, message:{message}");
        }
    }
}
