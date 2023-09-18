namespace WSC.UNITY.DEMO
{
    public class AnswerWSCTime : AnswerWSC
    {
        public double timestamp;

        internal override void OnQuery(Request request)
        {
            base.OnQuery(request);

            if (error != 0)
            {
                Log.Warning($"error: {error}");
                return;
            }

            WSCInfo.Time = Tools.UnixTime(timestamp);
        }
    }
}
