using System.Runtime.CompilerServices;

namespace WSC
{
    public class RequestWSCTime : RequestWSC<RequestWSCTime>
    {
        [ModuleInitializer]
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
