using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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

            WSC.Tools.JSON = JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true
                    }
                },
            });
            WSC.Tools.JSON.Converters.Add(new JavaScriptDateTimeConverter());
            WSC.Tools.JSON.NullValueHandling = NullValueHandling.Ignore;

            WSC.UNITY.Network.i.Initialize();
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
