using System;
using System.Collections.Generic;

namespace WSC
{
    public sealed class NetworkTask
    {
        private List<Request> requests = new List<Request>();
        private NetworkException exception = null;

        public NetworkTask Bind(Request request)
        {
            requests.Add(request);
            request.Done += (answer) =>
            {
                requests.Remove(request);

                if (answer.error != 0)
                    exception = new NetworkException(answer.error);

                if (requests.Count == 0)
                    Done?.Invoke(exception);
            };
            return this;
        }

        public event Action<NetworkException> Done = null;
    }
}