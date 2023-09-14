#pragma warning disable SYSLIB0014

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using static WSC.WebRequestTypes;

namespace WSC
{
    public sealed class WebRequest : IWeb
    {
        public void Query(RequestW3 request, Action<NetworkResponse> callback)
        {
            Task.Run(() =>
            {
                NetworkResponse result = null;

                try
                {
                    Log.Debug($"WWW request, method: {request.method}, uri: {request.uri}, {Tools.ToJson(request)}");

                    var www = WWW(request);
                    var response = (HttpWebResponse)www.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new NetworkException((int)response.StatusCode, response.StatusDescription);

                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            result = new NetworkResponse()
                            {
                                Exception = null,
                                Data = reader.ReadToEnd(),
                                Sender = this
                            };
                        }
                    }
                }
                catch (WebException e)
                {
                    int error = (e.Status == WebExceptionStatus.Success) ? (int)NetworkError.Network : (int)e.Status;
                    result = new NetworkResponse()
                    {
                        Exception = new NetworkException(error, e.ToString()),
                        Sender = this
                    };
                }
                catch (NetworkException e)
                {
                    result = new NetworkResponse()
                    {
                        Exception = e,
                        Sender = this
                    };
                }
                catch (Exception e)
                {
                    result = new NetworkResponse()
                    {
                        Exception = new NetworkException(NetworkError.Network, e.ToString()),
                        Sender = this
                    };
                }
                finally
                {
                    if (result.Exception != null)
                        Log.Debug($"WWW response uri: {request.uri}, error: {result.Exception.HResult}, message: {result.Exception.Message}");
                    else
                        Log.Debug($"WWW response uri: {request.uri}, contents: {result.Data}");

                    callback?.Invoke(result);
                }
            });
        }

        private HttpWebRequest WWW(RequestW3 payload)
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

        private HttpWebRequest Get(RequestW3 payload)
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

            HttpWebRequest www = (HttpWebRequest)System.Net.WebRequest.Create(uri);
            www.Method = GET;
            return www;
        }

        private HttpWebRequest Delete(RequestW3 payload)
        {
            HttpWebRequest www = (HttpWebRequest)System.Net.WebRequest.Create(payload.uri);
            www.Method = DELETE;
            return www;
        }

        private HttpWebRequest Put(RequestW3 payload)
        {
            var data = Encoding.UTF8.GetBytes(Tools.ToJson(payload));
            var www = (HttpWebRequest)System.Net.WebRequest.Create(payload.uri);
            www.ContentType = "application/json";
            www.ContentLength = data.Length;
            www.Method = PUT;

            using (Stream stream = www.GetRequestStream())
                stream.Write(data, 0, data.Length);

            return www;
        }

        private HttpWebRequest Post(RequestW3 payload)
        {
            var data = Encoding.UTF8.GetBytes(Tools.ToJson(payload));
            var www = (HttpWebRequest)System.Net.WebRequest.Create(payload.uri);
            www.ContentType = "application/json";
            www.ContentLength = data.Length;
            www.Method = POST;

            using (Stream stream = www.GetRequestStream())
                stream.Write(data, 0, data.Length);

            return www;
        }
    }
}
