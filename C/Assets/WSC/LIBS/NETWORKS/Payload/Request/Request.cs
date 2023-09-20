using System;
using Newtonsoft.Json;

namespace WSC
{
    public class Request
    {
        public virtual void Query()
        {
        }

        public virtual Request Query<T>(Action<T> callback = null) where T : Answer
        {
            return this;
        }

        public virtual Answer OnQuery(object sender)
        {
            return null;
        }

        [JsonIgnore] public Action<Answer> Done = delegate { };
        [JsonIgnore] public string host = string.Empty;
    }
}