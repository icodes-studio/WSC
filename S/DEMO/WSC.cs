using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WSC
{
    class WSC : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Log.Debug($"{ID}");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Log.Debug($"{ID}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText == true)
            {
                RequestWS request = null;
                AnswerWS answer = null;
                try
                {
                    request = Tools.FromJson<RequestWS>(e.Data);
                    if (request == null)
                    {
                        throw new NetworkException(NetworkError.InvalidData);
                    }
                    else
                    {
                        var handler = Program.i.FindHandler(request.command);
                        if (handler != null)
                        {
                            request =
                                typeof(Tools)
                                .GetMethod("FromJson")
                                .MakeGenericMethod(handler)
                                .Invoke(null, new object[] { e.Data }) as RequestWS;

                            Log.Debug($"command: {request.command}, contents: {e.Data}");

                            answer = (AnswerWS)request.OnQuery(this);
                        }
                        else
                        {
                            throw new NetworkException(NetworkError.CommandNotFound);
                        }
                    }
                }
                catch (NetworkException exception)
                {
                    answer = new AnswerWS() { error = exception.HResult };
                }
                catch (Exception)
                {
                    answer = new AnswerWS() { error = (int)NetworkError.InternalError };
                }
                finally
                {
                    if (answer == null)
                        answer = new AnswerWS() { error = (int)NetworkError.InternalError };

                    answer.index = request.index;

                    var response = Tools.ToJson(answer);
                    Log.Debug($"response: {response}");
                    Send(response);
                }
            }
        }
    }
}