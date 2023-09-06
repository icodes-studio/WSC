using System;
using UnityEngine;

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
        public static string Platform => Application.platform.ToString();
        public static string DeviceModel => SystemInfo.deviceModel;
        public static string DeviceID => SystemInfo.deviceUniqueIdentifier;
        public static string Locale => Application.systemLanguage.ToString();
        public static string Version => Application.version;
        public static string Host => "localhost:4649/WSC/";
    }
}