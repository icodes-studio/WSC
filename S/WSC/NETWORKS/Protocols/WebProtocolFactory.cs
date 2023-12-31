﻿using System;
using System.Collections.Generic;

namespace WSC
{
    public interface IWebRequest
    {
        void Query(RequestW3 request, Action<NetworkResponse> callback);
    }

    public interface IWebSocket
    {
        event Action<ushort> OnClose;
        event Action<string> OnError;
        event Action<string> OnMessage;
        event Action OnOpen;

        void Connect();
        void Send(string message, Action<bool> result);
        void Close();
        void Dispatch();

        Uri Uri { get; }
    }

    public interface IWebProtocolFactory
    {
        IWebRequest CreateWebRequest();
        IWebSocket CreateWebSocket(string uri, Dictionary<string, string> cookies);
    }

    public sealed class WebProtocolFactory : IWebProtocolFactory
    {
        public IWebRequest CreateWebRequest() => 
            new WSC.WebRequest();

        public IWebSocket CreateWebSocket(string uri, Dictionary<string, string> cookies) => 
            new WSC.WebSocket(uri, cookies);
    }
}
