#if UNITY_5_6_OR_NEWER
#define UNITY
#endif

using System;
using System.Text;
using System.Collections.Generic;

#if UNITY
using UnityEngine.Networking;
#else
using System.IO;
using System.Net;
#pragma warning disable SYSLIB0014
#endif

namespace WSC
{
    public static class NetworkW3
    {
        public const string GET = "GET";
        public const string PUT = "PUT";
        public const string POST = "POST";
        public const string DELETE = "DELETE";

#if UNITY
        public static UnityWebRequest Request(RequestW3 payload)
        {
            var www = payload.method switch
            {
                GET => Get(payload),
                DELETE => Delete(payload),
                PUT => Put(payload),
                POST => Post(payload),
                _ => null
            };

            if (payload.headers != null)
            {
                foreach (var header in payload.headers)
                    www.SetRequestHeader(header.Key, header.Value);
            }

            return www;
        }

        private static UnityWebRequest Get(RequestW3 payload)
        {
            var uri = payload.uri;
            var parameters = Tools.FromJson<Dictionary<string, string>>(Tools.ToJson(payload));
            if (parameters != null && parameters.Count > 0)
            {
                uri += uri.Contains("?") ? "&" : "?";

                foreach (var parameter in parameters)
                {
                    uri += string.Format("{0}={1}&",
                        Uri.EscapeDataString(parameter.Key),
                        parameter.Value == null ? string.Empty : Uri.EscapeDataString(parameter.Value));
                }
            }

            return new UnityWebRequest(uri, GET, new DownloadHandlerBuffer(), null);
        }

        private static UnityWebRequest Delete(RequestW3 payload)
        {
            return new UnityWebRequest(payload.uri, DELETE, new DownloadHandlerBuffer(), null);
        }

        private static UnityWebRequest Put(RequestW3 payload)
        {
            return new UnityWebRequest(payload.uri, PUT, new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(Tools.ToJson(payload))));
        }

        private static UnityWebRequest Post(RequestW3 payload)
        {
            return new UnityWebRequest(payload.uri, POST, new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(Tools.ToJson(payload))));
        }
#else
        public static HttpWebRequest Request(RequestW3 payload)
        {
            HttpWebRequest www = payload.method switch
            {
                GET => Get(payload),
                DELETE => Delete(payload),
                PUT => Put(payload),
                POST => Post(payload),
                _ => null
            };

            if (payload.headers != null)
            {
                foreach (var header in payload.headers)
                    www.Headers.Add(header.Key, header.Value);
            }

            return www;
        }

        private static HttpWebRequest Get(RequestW3 payload)
        {
            var uri = payload.uri;
            var parameters = Tools.FromJson<Dictionary<string, string>>(Tools.ToJson(payload));
            if (parameters != null && parameters.Count > 0)
            {
                uri += uri.Contains("?") ? "&" : "?";

                foreach (var parameter in parameters)
                {
                    uri += string.Format("{0}={1}&",
                        Uri.EscapeDataString(parameter.Key),
                        (parameter.Value == null) ? string.Empty : Uri.EscapeDataString(parameter.Value));
                }
            }

            HttpWebRequest www = (HttpWebRequest)WebRequest.Create(uri);
            www.Method = GET;
            return www;
        }

        private static HttpWebRequest Delete(RequestW3 payload)
        {
            HttpWebRequest www = (HttpWebRequest)WebRequest.Create(payload.uri);
            www.Method = DELETE;
            return www;
        }

        private static HttpWebRequest Put(RequestW3 payload)
        {
            var data = Encoding.UTF8.GetBytes(Tools.ToJson(payload));
            var www = (HttpWebRequest)WebRequest.Create(payload.uri);
            www.ContentType = "application/json";
            www.ContentLength = data.Length;
            www.Method = PUT;

            using (Stream stream = www.GetRequestStream())
                stream.Write(data, 0, data.Length);

            return www;
        }

        private static HttpWebRequest Post(RequestW3 payload)
        {
            var data = Encoding.UTF8.GetBytes(Tools.ToJson(payload));
            var www = (HttpWebRequest)WebRequest.Create(payload.uri);
            www.ContentType = "application/json";
            www.ContentLength = data.Length;
            www.Method = POST;

            using (Stream stream = www.GetRequestStream())
                stream.Write(data, 0, data.Length);

            return www;
        }
#endif
    }
}
