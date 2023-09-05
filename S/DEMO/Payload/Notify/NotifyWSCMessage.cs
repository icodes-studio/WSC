using System.Runtime.CompilerServices;

namespace WSC
{
    public class NotifyWSCMessage : NotifyWSC<NotifyWSCMessage>
    {
        public long type;
        public string message;

        [ModuleInitializer]
        internal static void Initialize()
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
