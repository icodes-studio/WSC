using Newtonsoft.Json;

namespace WSC
{
    public class Notify : Answer
    {
        public string command = string.Empty;
        [JsonIgnore] internal string host = string.Empty;
    }
}