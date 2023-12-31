﻿namespace WSC.DEMO
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
        private static void Initialize()
        {
            Register();
        }

        public override Answer OnQuery(object sender)
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
