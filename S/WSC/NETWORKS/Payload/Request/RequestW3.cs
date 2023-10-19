using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WSC
{
    public class RequestW3 : Request
    {
        public override Request Query()
        {
            NetworkW3Client.i.Query(this);
            return this;
        }

        public override Request Query<T>(Action<T> callback = null)
        {
            if (callback != null)
                Done += new Action<Answer>(answer => callback((T)answer));

            NetworkW3Client.i.Query<T>(this, (answer) =>
            {
                try
                {
                    answer.OnQuery(this);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    answer.error = (int)NetworkError.Exception;
                }

                try
                {
                    Done(answer);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });

            return this;
        }

        [JsonIgnore] public string command { get; set; } = string.Empty;
        [JsonIgnore] public string method { get; set; } = WebRequest.POST;
        [JsonIgnore] public string uri => host + command;
        [JsonIgnore] public Dictionary<string, string> headers = null;
        [JsonIgnore] public int recovery = NetworkTypes.RecoveryCount;
    }
}