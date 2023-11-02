using System;
using System.Collections.Generic;

namespace WSC
{
    public sealed class NetworkWSClient : Singleton<NetworkWSClient>
    {
        private IWebProtocolFactory factory = new WebProtocolFactory();
        private Dictionary<string, NetworkWS> sockets = new Dictionary<string, NetworkWS>();
        public event Action<NetworkResponse> OnMessage = delegate { };
        public event Action<NetworkResponse> OnClose = delegate { };
        public event Action<NetworkResponse> OnRestore = delegate { };
        public event Action<NetworkResponse> OnOpen = delegate { };

        public void Initialize(IWebProtocolFactory factory = null)
        {
            if (factory != null)
                this.factory = factory;
        }

        internal NetworkWSClient Query(RequestWS payload)
        {
            OnQuery(payload, null);
            return this;
        }

        internal NetworkWSClient Query<T>(RequestWS request, Action<T> callback) where T : Answer
        {
            OnQuery(request, (response) =>
            {
                if (response.Exception == null)
                {
                    if (callback != null)
                    {
                        T answer = default;
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
                                answer.error = (int)NetworkError.InvalidData;
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

        private void OnQuery(RequestWS request, Action<NetworkResponse> callback)
        {
            Log.Debug($"WEBSOCKET request:{Tools.ToJson(request)}");

            Socket(request.host, request.cookies).Send(
                Tools.ToJson(request),
                request.index,
                request.recovery,
                response =>
                {
                    Log.Debug($"WEBSOCKET response: {response.Data}, error: {response.Exception?.HResult}");
                    callback?.Invoke(response);
                });
        }

        private void OnNotify(NetworkResponse response)
        {
            if (response.Exception == null)
            {
                Notify notify = null;
                try
                {
                    notify = typeof(Tools)
                        .GetMethod("FromJson")
                        .MakeGenericMethod(response.DataType)
                        .Invoke(null, new object[] { response.Data }) as Notify;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
                finally
                {
                    if (notify != null)
                    {
                        Log.Debug($"WEBSOCKET notify: {response.Data}");
                        notify.host = (response.Sender as NetworkWS)?.Uri.ToString() ?? string.Empty;
                        notify.OnQuery(null);
                    }
                }
            }
        }

        private NetworkWS Socket(string host, Dictionary<string, string> cookies)
        {
            if (sockets.TryGetValue(host, out var socket) == false)
            {
                socket = new NetworkWS(host, cookies, factory);
                socket.OnNotify += OnNotify;
                socket.OnOpen += (response) => OnOpen(response);
                socket.OnRestore += (response) => OnRestore(response);
                socket.OnClose += (response) => OnClose(response);
                socket.OnMessage += (response) => OnMessage(response);
                sockets.Add(host, socket);
            }
            return socket;
        }

        internal NetworkWSClient RegisterNotify(Type type)
        {
            var notify = Activator.CreateInstance(type) as Notify;
            Socket(notify.host, notify.cookies).RegisterNotify(type);
            return this;
        }

        internal NetworkWSClient RegisterNotify(Type type, string command)
        {
            var notify = Activator.CreateInstance(type) as Notify;
            Socket(notify.host, notify.cookies).RegisterNotify(type, command);
            return this;
        }

        internal NetworkWSClient RegisterNotify(Type type, string host, string command)
        {
            var notify = Activator.CreateInstance(type) as Notify;
            Socket(host, notify.cookies).RegisterNotify(type, command);
            return this;
        }

        internal NetworkWSClient RegisterNotify(Type type, string host, Dictionary<string, string> cookies, string command)
        {
            Socket(host, cookies).RegisterNotify(type, command);
            return this;
        }

        public void Update()
        {
            var backup = new Dictionary<string, NetworkWS>(sockets);
            foreach (var socket in backup.Values)
                socket.Dispatch();
        }

        public NetworkWSClient Connect(string host, Dictionary<string, string> cookies = null)
        {
            Socket(host, cookies).Connect();
            return this;
        }

        public bool IsConnected(string host)
        {
            return (sockets.TryGetValue(host, out var socket)) ? socket.State == NetworkWS.STATE.Connected : false;
        }

        public NetworkWSClient Close(string host)
        {
            if (sockets.TryGetValue(host, out var socket))
                socket.Close();

            return this;
        }

        public NetworkWSClient CloseAll()
        {
            var backup = new Dictionary<string, NetworkWS>(sockets);
            foreach (var socket in backup.Values)
                socket.Close();

            return this;
        }

        protected override void OnDestroy()
        {
            CloseAll();
            base.OnDestroy();
        }
    }
}
