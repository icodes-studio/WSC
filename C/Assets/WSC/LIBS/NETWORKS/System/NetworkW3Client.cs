#if UNITY_5_6_OR_NEWER
#define UNITY
#endif

using System;

#if UNITY
using System.Collections;
using UnityEngine.Networking;
#else
using System.IO;
using System.Net;
using System.Threading.Tasks;
#endif

namespace WSC
{
    public sealed class NetworkW3Client : System<NetworkW3Client>
    {
        public NetworkW3Client Initialize()
        {
            return this;
        }

        public void Query(RequestW3 request)
        {
            Query(request, null);
        }

        public void Query<T>(RequestW3 request, Action<T> callback) where T : Answer
        {
            Query(request, (response) =>
            {
                if (response.Exception == null)
                {
                    if (callback != null)
                    {
                        T answer = null;
                        try
                        {
                            answer = Tools.FromJson<T>(response.Data);
                        }
                        catch (Exception e)
                        {
                            Log.Warning(e);
                            answer = Activator.CreateInstance<T>();
                            answer.error = (int)NetworkError.InvalidData;
                        }
                        finally
                        {
                            if (answer == null)
                            {
                                answer = Activator.CreateInstance<T>();
                                answer.error = (int)NetworkError.Unknown;
                            }
                            callback(answer);
                        }
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        T answer = Activator.CreateInstance<T>();
                        answer.error = response.Exception.HResult;
                        callback(answer);
                    }
                }
            });
        }

        private void Query(RequestW3 request, Action<NetworkResponse> callback)
        {
#if UNITY
            StartCoroutine(OnQuery(request, callback));
#else
            OnQuery(request, callback);
#endif
        }

#if UNITY
        private IEnumerator OnQuery(RequestW3 request, Action<NetworkResponse> callback)
        {
            Log.Debug($"WWW request, uri: {request.uri}, {Tools.ToJson(request)}");

            using (var www = NetworkW3.Request(request))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Log.Debug($"WWW response error, uri: {request.uri}, error: {www.responseCode}");

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
#else
        private void OnQuery(RequestW3 request, Action<NetworkResponse> callback)
        {
            Task.Run(() =>
            {
                NetworkResponse result = null;

                try
                {
                    Log.Debug($"WWW request, uri: {request.uri}, {Tools.ToJson(request)}");

                    var www = NetworkW3.Request(request);
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
                        Log.Debug($"WWW response uri: {request.uri}, error: {result.Exception.HResult}");
                    else
                        Log.Debug($"WWW response uri: {request.uri}, contents: {result.Data}");

                    callback?.Invoke(result);
                }
            });
        }
#endif
    }
}
