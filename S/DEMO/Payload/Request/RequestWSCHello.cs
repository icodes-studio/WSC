using System.Runtime.CompilerServices;

namespace WSC
{
    public class RequestWSCHello : RequestWSC<RequestWSCHello>
    {
        public string message;

        [ModuleInitializer]
        internal static void Initialize()
        {
            Register();
        }

        public override Answer OnQuery(object sender)
        {
            Log.Debug($"{nameof(message)}:{message}");

            return new AnswerWSCHello()
            {
                message = message,
            };
        }
    }
}
