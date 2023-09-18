namespace WSC.UNITY.DEMO
{
    public class AnswerWSCLogin : AnswerWSC
    {
        public string token;
        public double timestamp;

        public override void OnQuery(Request request)
        {
            base.OnQuery(request);

            if (error != 0)
            {
                Log.Warning($"error: {error}");
                return;
            }

            WSCInfo.AccessToken = token;
            WSCInfo.Time = Tools.UnixTime(timestamp);
        }
    }
}
