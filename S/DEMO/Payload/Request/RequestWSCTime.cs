using WSC;

namespace DEMO
{
    public class RequestWSCTime : RequestWSC<RequestWSCTime>
    {
        [RuntimeInitialize]
        internal static void Initialize()
        {
            Register();
        }

        public override Answer OnQuery(object sender)
        {
            return new AnswerWSCTime()
            {
                timestamp = Tools.UnixTime()
            };
        }
    }
}
