using UnityEngine;

namespace WSC.UNITY.DEMO
{
    public class WSC : MonoBehaviour
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

            Network.i.Initialize(new global::WSC.UNITY.WebProtocolFactory());
        }

        private void Start()
        {
            NetworkWSClient.i.OnOpen += (response) =>
            {
                Log.Warning($"OnOpen");
            };

            NetworkWSClient.i.OnRestore += (response) =>
            {
                Log.Warning($"OnRestore retry login");
                new RequestWSCLogin().Query();
            };

            NetworkWSClient.i.OnClose += (response) =>
            {
                Log.Warning($"OnClose retry login");
                new RequestWSCLogin().Query();
            };

            new RequestWSCLogin().Query();
            new RequestWSCLogin().Query<AnswerWSCLogin>((answer) =>
            {
                Log.Debug($"[TEST#1] timestamp: {answer.timestamp}, token: {answer.token}");
            });

            new RequestWSCTime().Query<AnswerWSCTime>().Done += ((answer) =>
            {
                if (answer is AnswerWSCTime time)
                    Log.Debug($"[TEST#2] timestamp: {time.timestamp}");
            });

            new RequestWSCHello() { message = "Hello" }.Query<AnswerWSCHello>((answer) =>
            {
                Log.Debug($"[TEST#3] message: {answer.message}");
            });

            new RequestWSCLogin().Query<Answer>((answer) =>
            {
                if (answer.error == 0)
                {
                    new RequestWSCTime().Query<Answer>((answer) =>
                    {
                        if (answer.error == 0)
                        {
                            new RequestWSCHello() { message = "Hello" }.Query<Answer>((answer) =>
                             {
                                 Log.Debug($"[TEST#4] error: {answer.error}");
                             });
                        }
                    });
                }
            });

            new NetworkTask()
                .Bind(new RequestWSCLogin().Query<AnswerWSCLogin>())
                .Bind(new RequestWSCTime().Query<AnswerWSCTime>())
                .Bind(new RequestWSCHello() { message = "Hello" }.Query<AnswerWSCHello>())
                .Done += exception =>
                {
                    Log.Debug($"TEST#5 error message: {exception?.ToString()}");
                };
        }
    }
}
