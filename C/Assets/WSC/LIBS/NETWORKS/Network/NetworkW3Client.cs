using System;

namespace WSC
{
    public sealed class NetworkW3Client : System<NetworkW3Client>
    {
        private IWebRequest www = null;

        public void Initialize()
        {
            www = Network.Factory.CreateWebRequest();
        }

        public NetworkW3Client Query(RequestW3 request)
        {
            www?.Query(request, null);
            return this;
        }

        public NetworkW3Client Query<T>(RequestW3 request, Action<T> callback) where T : Answer
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

            return this;
        }
    }
}
