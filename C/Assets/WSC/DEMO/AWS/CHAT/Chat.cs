using UnityEngine;
using WSC;

namespace DEMO.AWS.CHAT
{
    public class Chat : MonoBehaviour
    {
        private void Awake()
        {
            Log.Initialize(string.Empty, string.Empty, LogLevel.Debug, (level, message) =>
            {
                switch (level)
                {
                    case LogLevel.Debug:
                    case LogLevel.Information:
                    case LogLevel.Message:
                        Debug.Log(message);
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        Debug.LogError(message);
                        break;
                    case LogLevel.Fatal:
                        Debug.LogAssertion(message);
                        break;
                }
            });

            WSC.Network.Initialize();
            //WSC.Network.Initialize(new WSC.WebProtocolFactory());
            //WSC.Network.Initialize(new WSC.UNITY.WebProtocolFactory());
        }

        private void Start()
        {
            NetworkWSClient.i.Connect(ChatInfo.WSHost).OnOpen += (response) =>
            {
                Log.Debug($"Connected Socket({response.Data})");

                new RequestChatHistory().Query<AnswerChatHistory>((answer) =>
                {
                    foreach (var message in answer.messages)
                        Log.Debug($"[History] roomId:{message.roomId}, userId:{message.userId}, message:{message.message}, timestamp:{message.timestamp}");

                    new RequestChatMessage() { text = "Hello World" }.Query<AnswerChatMessage>((answer) =>
                        Log.Debug("[Send] Transfer completed."));
                });
             };

            NotifyChatMessage.OnNotify += (notify) =>
            {
                Log.Debug($"[Message] from:{notify.host}, roomId:{notify.roomId}, userId:{notify.userId}, name:{notify.name}, message:{notify.message}, timestamp:{notify.timestamp}");
            };

            NetworkWSClient.i.OnClose += (response) =>
            {
                Log.Debug($"Closed Socket({response.Data})");
            };
        }
    }
}
