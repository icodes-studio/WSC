namespace WSC.DEMO
{
    public class RequestWSCTime : RequestWSC<RequestWSCTime>
    {
        [RuntimeInitialize]
        private static void Initialize()
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
