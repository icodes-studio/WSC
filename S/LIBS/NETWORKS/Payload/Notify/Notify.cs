using System.Collections.Generic;
using Newtonsoft.Json;

namespace WSC
{
    public class Notify : Answer
    {
        public string command = string.Empty;
        [JsonIgnore] public string host = string.Empty;
        [JsonIgnore] public Dictionary<string, string> cookies = null;
    }
}