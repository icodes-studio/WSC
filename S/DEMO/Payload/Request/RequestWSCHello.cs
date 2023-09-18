using WSC;

namespace DEMO
{
    public class RequestWSCHello : RequestWSC<RequestWSCHello>
    {
        public string message;

        [RuntimeInitialize]
        internal static void Initialize()
        {
            Register();
        }

        internal override Answer OnQuery(object sender)
        {
            Log.Debug($"{nameof(message)}:{message}");

            return new AnswerWSCHello()
            {
                message = message,
            };
        }
    }
}
