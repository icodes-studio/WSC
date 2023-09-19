using System;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace WSC
{
    public static class Tools
    {
        private static JsonSerializer JSON = null;

        static Tools()
        {
            JSON = JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true
                    }
                },
            });
            JSON.Converters.Add(new JavaScriptDateTimeConverter());
            JSON.NullValueHandling = NullValueHandling.Ignore;
        }

        public static string ToJson(object obj)
        {
            var write = new StringWriter();
            using (var writer = new JsonTextWriter(write))
                JSON.Serialize(writer, obj);

            return write.ToString();
        }

        public static T FromJson<T>(string json)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
                return JSON.Deserialize<T>(reader);
        }

        public static DateTime UnixTime(double timeStamp) =>
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();

        public static DateTime JavaTime(double timeStamp) =>
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeStamp).ToLocalTime();

        public static double UnixTime() =>
            DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static double JavaTime() =>
            DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        public static DateTime ToLocalTime(long time) =>
            ToLocalTime(time.ToString());

        public static DateTime ToLocalTime(string time) =>
            ToDateTime(time).ToLocalTime();

        public static DateTime ToLocalTime(DateTime value) =>
            value.Kind switch
            {
                DateTimeKind.Unspecified => new DateTime(value.Ticks, DateTimeKind.Local),
                DateTimeKind.Utc => value.ToLocalTime(),
                DateTimeKind.Local => value,
                _ => value,
            };

        public static DateTime ToUniversalTime(DateTime value) =>
            value.Kind switch
            {
                DateTimeKind.Unspecified => new DateTime(value.Ticks, DateTimeKind.Utc),
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => value,
            };

        public static DateTime ToDateTime(long time) =>
            ToDateTime(time.ToString());

        public static DateTime ToDateTime(string time) =>
            DateTime.ParseExact(time, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        public static string TimeToString(DateTime time) =>
            time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        public static long TimeToLong(DateTime time) =>
            Int64.Parse(time.ToString("yyyyMMddHHmmss"));

        public static string FullPath(string path) =>
            AppDomain.CurrentDomain.BaseDirectory + path;
    }
}
