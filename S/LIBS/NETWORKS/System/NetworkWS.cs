using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WSC
{
    public class NetworkWS
    {
        private int recoveryCount = NetworkTypes.RECOVERY;
        private IWebSocket socket = null;
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

        public NetworkWS(string host, IWebSocketFactory factory)
        {
            socket = factory.Create(host);
            socket.OnOpen += OnSocketOpen;
            socket.OnMessage += OnSocketMessage;
            socket.OnError += OnSocketError;
            socket.OnClose += OnSocketClose;
        }

        protected virtual void OnSocketOpen()
        {
            lock (sync)
            {
                Log.Debug($"Socket({socket.Uri}) opend");

                State = STATE.Connected;

                foreach (var pending in pendings)
                    Send(pending.request, pending.index, false, pending.callback);

                pendings.Clear();

                messages.Enqueue(new Message()
                {
                    callback = OnOpen,
                    response = new NetworkResponse()
                    {
                        Data = socket.Uri.ToString(),
                        Sender = this
                    }
                });

                recoveryCount = NetworkTypes.RECOVERY;
            }
        }

        protected virtual void OnSocketMessage(string message)
        {
            if (OnHandleAnswer(message))
                return;

            if (OnHandleNotify(message))
                return;
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

                    var command = notify.command ?? string.Empty;
                    if (!notifications.TryGetValue(command, out var notification))
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

        protected virtual void OnSocketError(string message)
        {
            Log.Debug($"Socket({socket?.Uri}) error({message})");
        }

        protected virtual void OnSocketClose(ushort code)
        {
            lock (sync)
            {
                Log.Debug($"Socket({socket?.Uri}) closed({code})");

                State = STATE.Closed;

                if (code == (ushort)CloseStatusCode.Normal)
                {
                    // When explicitly closed
                }
                else
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
                                Exception = new NetworkException(code),
                                Data = socket.Uri.ToString(),
                                Sender = this
                            }
                        });
                    }
                }
            }
        }

        private async void Connect(bool recovering)
        {
            if (State == STATE.Closed)
            {
                State = STATE.Connecting;

                if (recovering == true)
                {
                    Log.Debug($"Retring({recoveryCount})... {socket?.Uri}");
                    recoveryCount = recoveryCount - 1;
                    await Task.Delay(1000);
                }
                else
                {
                    Log.Debug($"Connecting... {socket?.Uri}");
                    recoveryCount = NetworkTypes.RECOVERY;
                }

                socket?.Connect();
            }
        }

        public void Connect()
        {
            Connect(false);
        }

        public void Close()
        {
            pendings?.Clear();
            callbacks?.Clear();

            recoveryCount = 0;
            socket?.Close();
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

                try
                {
                    socket.Send(payload, result =>
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
            socket?.Dispatch();

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
