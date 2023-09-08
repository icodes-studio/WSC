using System;
using System.Collections.Generic;

namespace WSC
{
    public sealed class NetworkWSClient : System<NetworkWSClient>
    {
        private IWebSocketFactory factory = new WebSocketFactory();
        private Dictionary<string, NetworkWS> sockets = new Dictionary<string, NetworkWS>();
        public event Action<NetworkResponse> OnMessage = delegate { };
        public event Action<NetworkResponse> OnClose = delegate { };
        public event Action<NetworkResponse> OnOpen = delegate { };

        public void Initialize(IWebSocketFactory factory = null)
        {
            if (factory != null)
                this.factory = factory;
        }

        public NetworkWSClient Query(RequestWS payload)
        {
            OnQuery(payload, null);
            return this;
        }

        public NetworkWSClient Query<T>(RequestWS request, Action<T> callback) where T : Answer
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

            Socket(request.host).Send(
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
                var notify =
                    typeof(Tools)
                    .GetMethod("FromJson")
                    .MakeGenericMethod(response.DataType)
                    .Invoke(null, new object[] { response.Data }) as Notify;

                if (notify != null)
                {
                    Log.Debug($"WEBSOCKET notify: {response.Data}");
                    notify.OnQuery(null);
                }
            }
        }

        private NetworkWS Socket(string host)
        {
            if (sockets.TryGetValue(host, out var socket) == false)
            {
                socket = new NetworkWS(host, factory);
                socket.OnNotify += OnNotify;
                socket.OnOpen += (response) => OnOpen(response);
                socket.OnClose += (response) => OnClose(response);
                socket.OnMessage += (response) => OnMessage(response);
                sockets.Add(host, socket);
            }
            return socket;
        }

        public void Update()
        {
            var backup = new Dictionary<string, NetworkWS>(sockets);
            foreach (var socket in backup.Values)
                socket.Dispatch();
        }

        public NetworkWSClient RegisterNotify(Type type)
        {
            Socket((Activator.CreateInstance(type) as Notify).host).RegisterNotify(type);
            return this;
        }

        public NetworkWSClient RegisterNotify(Type type, string command)
        {
            Socket((Activator.CreateInstance(type) as Notify).host).RegisterNotify(type, command);
            return this;
        }

        public NetworkWSClient Connect(string host)
        {
            Socket(host).Connect();
            return this;
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
