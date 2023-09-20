using System;
using System.Collections.Concurrent;

namespace WSC
{
    public sealed class NetworkW3Client : Singleton<NetworkW3Client>
    {
        private IWebRequest www = new WebRequest();
        private ConcurrentQueue<Response> responses = new ConcurrentQueue<Response>();

        private class Response
        {
            public Delegate callback;
            public Answer answer;
        }

        public void Initialize(IWebProtocolFactory factory = null)
        {
            if (factory != null)
                www = factory.CreateWebRequest();
        }

        internal NetworkW3Client Query(RequestW3 request)
        {
            www?.Query(request, null);
            return this;
        }

        internal NetworkW3Client Query<T>(RequestW3 request, Action<T> callback) where T : Answer
        {
            www?.Query(request, (response) =>
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

                            responses.Enqueue(new Response()
                            {
                                callback = callback,
                                answer = answer
                            });
                        }
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        T answer = Activator.CreateInstance<T>();
                        answer.error = response.Exception.HResult;

                        responses.Enqueue(new Response()
                        {
                            callback = callback,
                            answer = answer
                        });
                    }
                }
            });

            return this;
        }

        public void Update()
        {
            while (responses.TryDequeue(out var response))
                response.callback?.DynamicInvoke(response?.answer);
        }
    }
}
