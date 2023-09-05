using Newtonsoft.Json;

namespace WSC
{
    public class Notify : Answer
    {
        public string command;
        [JsonIgnore] internal string host = string.Empty;
    }
}