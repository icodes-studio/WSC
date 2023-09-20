using WSC;

namespace DEMO
{
    public class RequestWSCHello : RequestWSC<RequestWSCHello>
    {
        public string message;

        [RuntimeInitialize]
        private static void Initialize()
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
