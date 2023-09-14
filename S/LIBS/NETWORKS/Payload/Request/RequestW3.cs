using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WSC
{
    public class RequestW3 : Request
    {
        public override void Query()
        {
            NetworkW3Client.i.Query(this);
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

        [JsonIgnore] internal string command { get; set; } = string.Empty;
        [JsonIgnore] internal string method { get; set; } = WebRequestTypes.POST;
        [JsonIgnore] internal string uri => host + command;
        [JsonIgnore] internal Dictionary<string, string> headers = null;
        [JsonIgnore] internal int recovery = NetworkTypes.RecoveryCount;
    }
}