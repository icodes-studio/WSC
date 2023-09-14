using System;
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

        public override void Query()
        {
            NetworkWSClient.i.Query(this);
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

        [JsonIgnore] internal bool recovery = true;
    }
}
