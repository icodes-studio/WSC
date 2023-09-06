using System;
using System.Threading;

namespace WSC.DEMO
{
    public static class WSCInfo
    {
        private static TimeSpan time = new TimeSpan();
        public static DateTime Time
        {
            get => DateTime.Now.Add(time);
            set => time = value - DateTime.Now;
        }

        public static string AccessToken { get; set; } = string.Empty;
        public static string Platform => Environment.OSVersion.Platform.ToString(); 
        public static string DeviceModel => Environment.OSVersion.ToString();
        public static string DeviceID => Environment.OSVersion.VersionString;
        public static string Locale => Thread.CurrentThread.CurrentCulture.Name;
        public static string Version => Environment.Version.ToString();
        public static string Host => "localhost:5000/WSC/";
    }
}