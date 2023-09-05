using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;

namespace WSC
{
    public class NetworkWS
    {
        private const int RECOVERY = 10;

        private int recoveryCount = RECOVERY;
        private WebSocket socket = null;
        private Queue<Pending> pendings = new Queue<Pending>();
        private Queue<Message> messages = new Queue<Message>();
        private Dictionary<string, Action<NetworkResponse>> callbacks = new Dictionary<string, Action<NetworkResponse>>();
        private Dictionary<string, Type> notifications = new Dictionary<string, Type>();
        private object sync = new object();

        public event Action<NetworkResponse> OnNotify = delegate { };
        public event Action<NetworkResponse> OnMessage = delegate { };
        public event Action<NetworkResponse> OnClose = delegate { };
        public event Action<NetworkResponse> OnOpen = delegate { };

        private class Pending
        {
            public string index;
            public string request;
            public Action<NetworkResponse> callback;
        }

        private class Message
        {
            public NetworkResponse response;
            public Action<NetworkResponse> callback;
        }

        public STATE State
        {
            get;
            private set;
        }

        public enum STATE
        {
            Closed = 0,
            Connecting,
            Connected,
            Error
        }

        public NetworkWS(string host)
        {
            socket = new WebSocket(host);
            socket.OnOpen += OnSocketOpen;
            socket.OnMessage += OnSocketMessage;
            socket.OnError += OnSocketError;
            socket.OnClose += OnSocketClose;

            if (socket.IsSecure == true)
                socket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
#if DEBUG
            socket.Log.Level = WebSocketSharp.LogLevel.Trace;
#endif
        }

        protected virtual void OnSocketOpen(object sender, EventArgs e)
        {
            lock (sync)
            {
                Log.Debug($"Socket({socket?.Url}) opend");

                State = STATE.Connected;

                foreach (var pending in pendings)
                    Send(pending.request, pending.index, false, pending.callback);

                pendings.Clear();

                messages.Enqueue(new Message()
                {
                    callback = OnOpen,
                    response = new NetworkResponse()
                    {
                        Data = socket.Url.ToString(),
                        Sender = this
                    }
                });
            }
        }

        protected virtual void OnSocketMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText == true)
            {
                if (OnHandleAnswer(e.Data))
                    return;

                if (OnHandleNotify(e.Data))
                    return;
            }
        }

        private bool OnHandleAnswer(string data)
        {
            try
            {
                lock (sync)
                {
                    var answer = Tools.FromJson<AnswerWS>(data);
                    if (answer == null)
                        return false;

                    var index = answer.index;
                    if (string.IsNullOrEmpty(index))
                        return false;

                    if (callbacks.TryGetValue(index, out var callback) == false)
                        return false;

                    messages.Enqueue(new Message()
                    {
                        callback = callback,
                        response = new NetworkResponse()
                        {
                            Data = data,
                            Sender = this,
                        }
                    });

                    callbacks.Remove(index);
                }
            }
            catch (Exception e)
            {
                Log.Warning(e);
                return false;
            }

            return true;
        }

        private bool OnHandleNotify(string data)
        {
            try
            {
                lock (sync)
                {
                    var notify = Tools.FromJson<Notify>(data);
                    if (notify == null)
                        return false;

                    var command = notify.command;
                    if (string.IsNullOrEmpty(command))
                        return false;

                    if (!notifications.TryGetValue(command, out var notification) && !notifications.TryGetValue(string.Empty, out notification))
                        return false;

                    messages.Enqueue(new Message()
                    {
                        response = new NetworkResponse()
                        {
                            Data = data,
                            DataType = notification,
                            Sender = this,
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Warning(e);
                return false;
            }

            return true;
        }

        protected virtual void OnSocketError(object sender, ErrorEventArgs e)
        {
            Log.Debug($"Socket({socket?.Url}) error({e?.Message})");
        }

        protected virtual void OnSocketClose(object sender, CloseEventArgs e)
        {
            lock (sync)
            {
                Log.Debug($"Socket({socket?.Url}) closed({e?.Code})");

                State = STATE.Closed;

                if (e == null || e.Code != (ushort)CloseStatusCode.NoStatus)
                {
                    foreach (var callback in callbacks)
                    {
                        messages.Enqueue(new Message()
                        {
                            callback = callback.Value,
                            response = new NetworkResponse()
                            {
                                Exception = new NetworkException(NetworkError.Closed),
                                Sender = this,
                            }
                        });
                    }

                    callbacks.Clear();

                    if (recoveryCount > 0)
                    {
                        Connect(true);
                    }
                    else
                    {
                        foreach (var pending in pendings)
                        {
                            messages.Enqueue(new Message()
                            {
                                callback = pending.callback,
                                response = new NetworkResponse()
                                {
                                    Exception = new NetworkException(NetworkError.Closed),
                                    Sender = this,
                                }
                            });
                        }
                        pendings.Clear();

                        messages.Enqueue(new Message()
                        {
                            callback = OnClose,
                            response = new NetworkResponse()
                            {
                                Exception = new NetworkException((e == null) ? (int)NetworkError.Closed : e.Code),
                                Data = socket.Url.ToString(),
                                Sender = this
                            }
                        });
                    }
                }
            }
        }

        private void Connect(bool recovering)
        {
            if (State == STATE.Closed)
            {
                if (recovering == true)
                    recoveryCount = recoveryCount - 1;
                else
                    recoveryCount = RECOVERY;

                State = STATE.Connecting;

                Log.Debug($"Connecting... {socket.Url}");
#if NETCOREAPP
                Task.Run(() => socket.Connect()).ContinueWith((task) =>
                {
                    if (task.IsCompleted)
                    {
                        if (task.IsFaulted)
                        {
                            Log.Warning(task.Exception);
                            OnSocketClose(socket, null);
                        }
                    }
                });
#else
                try
                {
                    socket.ConnectAsync();
                }
                catch (Exception e)
                {
                        Log.Warning(e);
                        OnSocketClose(socket, null);
                }
#endif
            }
        }

        public void Connect()
        {
            Connect(false);
        }

        public void Close()
        {
            pendings.Clear();
            callbacks.Clear();
            socket.CloseAsync();

            State = STATE.Closed;
        }

        public void Send(string payload)
        {
            Send(payload, string.Empty, true, null);
        }

        public void Send(string payload, string index, bool recovery, Action<NetworkResponse> callback)
        {
            if (State == STATE.Connected)
            {
                if (callback != null)
                    callbacks.Add(index, callback);

#if NETCOREAPP
                Task.Run(() => socket.Send(payload)).ContinueWith((task) =>
                {
                    if (task.IsCompleted)
                    {
                        if (task.IsFaulted)
                        {
                            Log.Warning(task.Exception);
                            callbacks.Remove(index);
                            OnSendFail(callback);
                        }
                    }
                });
#else
                try
                {
                    socket.SendAsync(payload, result =>
                    {
                        if (result == false)
                        {
                            callbacks.Remove(index);
                            OnSendFail(callback);
                        }
                    });
                }
                catch (Exception e)
                {
                    Log.Warning(e);
                    OnSendFail(callback);
                }
#endif
            }
            else if (recovery == true)
            {
                pendings.Enqueue(new Pending()
                {
                    index = index,
                    request = payload,
                    callback = callback
                });

                Connect(false);
            }
            else
            {
                OnSendFail(callback);
            }
        }

        private void OnSendFail(Action<NetworkResponse> callback)
        {
            if (callback != null)
            {
                messages.Enqueue(new Message()
                {
                    callback = callback,
                    response = new NetworkResponse()
                    {
                        Exception = new NetworkException(NetworkError.SendFailed),
                        Sender = this
                    }
                });
            }
        }

        public void Dispatch()
        {
            lock (sync)
            {
                foreach (var message in messages)
                {
                    if (message.callback == null)
                    {
                        if (message.response.DataType != null)
                            OnNotify(message.response);
                    }
                    else
                    {
                        message.callback(message.response);
                    }
                    OnMessage(message.response);
                }
                messages.Clear();
            }
        }

        public void RegisterNotify(Type type)
        {
            try
            {
                notifications.Add((Activator.CreateInstance(type) as Notify).command, type);
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
        }

        public void RegisterNotify(Type type, string command)
        {
            try
            {
                notifications.Add(command, type);
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
        }

        public void UnregisterNotify(Type type)
        {
            notifications.Remove((Activator.CreateInstance(type) as Notify).command);
        }

        public void UnregisterNotify(string command)
        {
            notifications.Remove(command);
        }
    }
}
