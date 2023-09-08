using UnityEngine;
using WSC;

namespace AWS.CHAT
{
    public class Chat : MonoBehaviour
    {
        private void Awake()
        {
            Application.runInBackground = true;

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

            //NetworkW3Client.i.Initialize();
            //NetworkWSClient.i.Initialize(new WSC.DEMO.WebSocketFactory());
            RuntimeInitializeAttribute.Initialize();
        }

        private void Start()
        {
            NetworkWSClient.i.Connect(ChatInfo.WSHost).OnOpen += (response) =>
            {
                new RequestChatHistory().Query<AnswerChatHistory>((answer) =>
                {
                    foreach (var message in answer.messages)
                        Log.Debug($"roomId:{message.roomId}, userId:{message.userId}, message:{message.message}, timestamp:{message.timestamp}");

                    new RequestChatMessage() { text = "Hello World" }.Query<AnswerChatMessage>((answer) =>
                        Log.Debug("Done"));
                });
             };
        }
    }
}
