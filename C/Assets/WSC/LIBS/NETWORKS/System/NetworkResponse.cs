using System;

namespace WSC
{
    public sealed class NetworkResponse
    {
        public NetworkException Exception { get; set; }
        public Type DataType { get; set; }
        public string Data { get; set; }
        public object Sender { get; set; }
    }
}