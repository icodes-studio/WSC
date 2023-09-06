namespace WSC.DEMO
{
    public class RequestWSCLogin : RequestWSC
    {
        public string udid;
        public string device;

        public RequestWSCLogin()
        {
            udid = WSCInfo.DeviceID;
            device = WSCInfo.DeviceModel;
        }
    }
}
