using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using static WSC.WebRequestTypes;

namespace WSC.DEMO
{
    public sealed class WebRequest : IWeb
    {
        public void Query(RequestW3 request, Action<NetworkResponse> callback)
        {
            NetworkW3Client.i.StartCoroutine(OnQuery(request, callback));
        }

        private IEnumerator OnQuery(RequestW3 request, Action<NetworkResponse> callback)
        {
            Log.Debug($"WWW request, method: {request.method}, uri: {request.uri}, {Tools.ToJson(request)}");

            using (var www = WWW(request))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Log.Debug($"WWW response error, uri: {request.uri}, error: {www.responseCode}, message {www.error}");

                    int error = (www.responseCode == 0) ? (int)NetworkError.Network : (int)www.responseCode;
                    callback?.Invoke(new NetworkResponse()
                    {
                        Exception = new NetworkException(error, www.error),
                        Sender = this
                    });
                }
                else
                {
                    Log.Debug($"WWW response, uri: {request.uri}, contents: {www.downloadHandler.text}");

                    callback?.Invoke(new NetworkResponse()
                    {
                        Data = www.downloadHandler.text,
                        Sender = this
                    });
                }
            }
        }

        private UnityWebRequest WWW(RequestW3 payload)
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

            www.SetRequestHeader("Content-Type", "application/json");

            return www;
        }

        private UnityWebRequest Get(RequestW3 payload)
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

        private UnityWebRequest Delete(RequestW3 payload)
        {
            return new UnityWebRequest(payload.uri, DELETE, new DownloadHandlerBuffer(), null);
        }

        private UnityWebRequest Put(RequestW3 payload)
        {
            return new UnityWebRequest(payload.uri, PUT, new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(Tools.ToJson(payload))));
        }

        private UnityWebRequest Post(RequestW3 payload)
        {
            return new UnityWebRequest(payload.uri, POST, new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(Tools.ToJson(payload))));
        }
    }
}
