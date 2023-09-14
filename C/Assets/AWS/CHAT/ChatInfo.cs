using System;
using System.Diagnostics;
using UnityEngine;

namespace AWS.CHAT
{
    public static class ChatInfo
    {
        const string RestID = "9o107n8j82";
        const string SocketID = "e1hp1224fj";

        public static string NAME => Environment.MachineName;
        public static string UID => Process.GetCurrentProcess().Id.ToString();
        public static string RID => Application.platform.ToString();
        public static string WSHost => $"wss://{SocketID}.execute-api.ap-northeast-2.amazonaws.com/dev/?userId={UID}&roomId={RID}";
        public static string W3Host => $"https://{RestID}.execute-api.ap-northeast-2.amazonaws.com/dev/chat/";
    }
}
