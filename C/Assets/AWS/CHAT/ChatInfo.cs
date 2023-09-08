using System;
using System.Diagnostics;
using UnityEngine;

namespace AWS.CHAT
{
    public static class ChatInfo
    {
        public static string NAME => SystemInfo.deviceName;
        public static string UID => Process.GetCurrentProcess().Id.ToString();
        public static string RID => Application.platform.ToString();
        public static string WSHost => $"wss://q0gbn9hv8a.execute-api.ap-northeast-2.amazonaws.com/dev/?userId={UID}&roomId={RID}";
        public static string W3Host => $"https://lnb9gjusv4.execute-api.ap-northeast-2.amazonaws.com/dev/chat/";
    }
}
