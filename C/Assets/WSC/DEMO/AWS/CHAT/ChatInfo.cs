using System;
using System.Diagnostics;
using UnityEngine;

namespace DEMO.AWS.CHAT
{
    public static class ChatInfo
    {
        const string RestID = "YOUR-REST_ID";
        const string SocketID = "YOUR-SOCKET-ID";

        public static string NAME => Environment.MachineName;
        public static string UID => Process.GetCurrentProcess().Id.ToString();
        public static string RID => Application.platform.ToString();
        public static string WSHost => $"wss://{SocketID}.execute-api.ap-northeast-2.amazonaws.com/dev/?user_id={UID}&room_id={RID}";
        public static string W3Host => $"https://{RestID}.execute-api.ap-northeast-2.amazonaws.com/dev/chat/";
    }
}
