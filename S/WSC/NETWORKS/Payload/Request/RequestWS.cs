using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WSC
{
    public class RequestWS : Request
    {
        public string command;
        public string index;

        public RequestWS()
        {
            command = string.Empty;
            index = Guid.NewGuid().ToString();
        }

        public override Request Query()
        {
            NetworkWSClient.i.Query(this);
            return this;
        }

        public override Request Query<T>(Action<T> callback = null)
        {
            if (callback != null)
                Done += new Action<Answer>(answer => callback((T)answer));

            NetworkWSClient.i.Query<T>(this, (answer) =>
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

        [JsonIgnore] public Dictionary<string, string> cookies = null;
        [JsonIgnore] public bool recovery = true;
    }
}
