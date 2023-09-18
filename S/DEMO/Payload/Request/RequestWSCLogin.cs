using WSC;

namespace DEMO
{
    public class RequestWSCLogin : RequestWSC<RequestWSCLogin>
    {
        public string udid;
        public string device;

        public RequestWSCLogin()
        {
            udid = WSCInfo.DeviceID;
            device = WSCInfo.DeviceModel;
        }

        [RuntimeInitialize]
        internal static void Initialize()
        {
            Register();
        }

        internal override Answer OnQuery(object sender)
        {
            Log.Debug($"{nameof(udid)}:{udid}, {nameof(device)}:{device}");

            return new AnswerWSCLogin()
            {
                token = "1234567890",
                timestamp = Tools.UnixTime()
            };
        }
    }
}
