using System;
using System.Collections.Generic;
using System.Threading;

namespace WSC.DEMO
{
    internal static class WSCInfo
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
        public static string DeviceName => Environment.MachineName;
        public static string Locale => Thread.CurrentThread.CurrentCulture.Name;
        public static string Version => Environment.Version.ToString();
        public static string Host => "localhost:4649/WSC/";
        public static string WSHost => $"ws://{WSCInfo.Host}?name={WSCInfo.DeviceName}";
        public static string W3Host => $"http://{WSCInfo.Host}";

        public static Dictionary<string, string> WSCookies =>
            new Dictionary<string, string>
            {
                { "Authorization", AccessToken },
                { "Device", DeviceModel }
            };

        public static Dictionary<string, string> W3Headers =>
            new Dictionary<string, string>
            {
                { "Authorization", string.Format("Bearer {0}", AccessToken) }
            };
    }
}