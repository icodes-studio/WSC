using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebSocketSharp.Server;

namespace WSC.DEMO
{
    class Program : Singleton<Program>
    {
        private object sync = new();
        private HttpServer server = null;
        private WebSocketSessionManager sessions = null;
        private Dictionary<string, Type> handlers = new();
        private AppConfig config = null;

        protected override void Awake()
        {
            base.Awake();

            Trace.Listeners.Add(new ConsoleTraceListener());
            config = Tools.FromJson<AppConfig>(File.ReadAllText(Tools.FullPath("S.Config.json")));
            Log.Initialize(config.LogPath, nameof(WSC), config.LogLevel, (_, message) => Trace.WriteLine(message));

            Network.i.Initialize();
        }

        public bool Start()
        {
            try
            {
                server = new HttpServer(config.Port);
                server.OnGet += OnRequestW3;
                server.OnPut += OnRequestW3;
                server.OnPost += OnRequestW3;
                server.OnDelete += OnRequestW3;

                server.AddWebSocketService<WSC>(config.Service);
                sessions = server.WebSocketServices[config.Service].Sessions;

                server.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if ((server?.IsListening).Value)
            {
                Log.Message($"LISTENING on port {server.Port}, and providing WebSocket services:");
                foreach (var path in server.WebSocketServices.Paths)
                    Log.Message($"- {path}");

                Log.Message($"The server has successfully started.");
                return true;
            }
            else
            {
                Log.Error($"\n\nThe Server failed to start.");
                return false;
            }
        }

        public void Stop()
        {
            server?.Stop();
            server = null;
        }

        private void OnRequestW3(object sender, HttpRequestEventArgs args)
        {
            Answer answer = null;
            try
            {
                var path = args.Request.RawUrl.TrimEnd('/');
                var index = path.LastIndexOf('/');
                var service = string.Empty;
                var command = path;
                var queries = string.Empty;

                if (index > 0)
                {
                    service = path.Substring(0, index);
                    command = path.Substring(index + 1);
                }

                if (service != config.Service)
                    throw new NetworkException(NetworkError.CommandNotFound);

                index = command.IndexOf('?');
                if (index > 0)
                {
                    queries = command.Substring(index + 1);
                    command = command.Substring(0, index);
                }

                var handler = FindHandler(command);
                if (handler != null)
                {
                    var content = string.Empty;
                    if (args.Request.HttpMethod == WebRequest.GET)
                    {
                        var parameters = HttpUtility.ParseQueryString(queries);
                        var payload = new StringBuilder("{");
                        foreach (var key in parameters.AllKeys)
                            payload.Append($@"""{key}"":""{parameters[key]}"",");
                        payload.Append("}");
                        content = payload.ToString();
                    }
                    else
                    {
                        using (var reader = new StreamReader(args.Request.InputStream))
                            content = reader.ReadToEnd();
                    }

                    Log.Debug($"command: {command}, method: {args.Request.HttpMethod}, contents: {content}");

                    var request =
                        typeof(Tools)
                        .GetMethod("FromJson")
                        .MakeGenericMethod(handler)
                        .Invoke(null, new object[] { content }) as RequestW3;

                    if (request != null)
                    {
                        request.method = args.Request.HttpMethod;
                        answer = request.OnQuery(null);
                    }
                    else
                    {
                        throw new NetworkException(NetworkError.InvalidData);
                    }
                }
                else
                {
                    throw new NetworkException(NetworkError.CommandNotFound);
                }
            }
            catch (NetworkException e)
            {
                answer = new Answer { error = e.HResult };
            }
            catch (Exception)
            {
                answer = new Answer { error = (int)NetworkError.InternalError };
            }
            finally
            {
                if (answer == null)
                    answer = new Answer { error = (int)NetworkError.InternalError };

                var response = Tools.ToJson(answer);
                var contents = Encoding.UTF8.GetBytes(response);
                args.Response.ContentType = "application/json";
                args.Response.ContentEncoding = Encoding.UTF8;
                args.Response.ContentLength64 = contents.LongLength;
                args.Response.Close(contents, true);

                Log.Debug($"response: {response}");
            }
        }

        public void RegisterHandler(string command, Type type)
        {
            lock (sync)
            {
                try
                {
                    handlers.Add(command, type);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public Type FindHandler(string command)
        {
            lock (sync)
            {
                Type handler = null;
                handlers.TryGetValue(command, out handler);
                return handler;
            }
        }

        public void Command(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    Console.WriteLine("Invalid command - Emtpy");
                    return;
                }

                string target = string.Empty;

                if (message[0] == '@')
                {
                    var s = message.IndexOf(' ');
                    target = message.Substring(1, s - 1);
                    message = message.Substring(s + 1);
                }

                if (message[0] == '{')
                {
                    var notify = Tools.FromJson<Notify>(message);
                    if (notify == null)
                    {
                        Console.WriteLine("Invalid command - JSON");
                        return;
                    }
                }
                else
                {
                    var notify = Activator.CreateInstance(Type.GetType(message)) as Notify;
                    if (notify == null)
                    {
                        Console.WriteLine("Invalid command - Type");
                        return;
                    }

                    message = Tools.ToJson(notify);
                }

                Console.WriteLine("Sending...");

                if (string.IsNullOrEmpty(target))
                    Broadcast(message, () => Console.WriteLine("Transfer completed"));
                else
                    SendTo(target, message, () => Console.WriteLine("Transfer completed"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Invalid command - Exception");
            }
        }

        public void Broadcast(string message, Action completed)
        {
            Log.Debug($"{nameof(message)}: {message}");

            try
            {
                sessions.BroadcastAsync(message, () =>
                {
                    completed?.Invoke();
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void SendTo(string id, string message, Action completed)
        {
            Log.Debug($"{nameof(id)}: {id}, {nameof(message)}: {message}");

#if NETCOREAPP
            Task.Run(() => sessions.SendTo(message, id)).ContinueWith((task) =>
            {
                if (task.IsCompletedSuccessfully)
                    completed?.Invoke();
                else
                    Log.Error(task.Exception);
            });
#else
            try
            {
                sessions.SendToAsync(message, id, (result) => completed?.Invoke());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
#endif
        }

        static void Main(string[] args)
        {
            if (i.Start() == false)
                return;

            Console.WriteLine("\nType 'exit' to stop the server.\n");

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                {
                    i.Stop();
                    break;
                }
                else
                {
                    i.Command(command);
                }
            }
        }
    }
}