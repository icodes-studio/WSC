using System;

namespace WSC
{
    public class NetworkException : Exception
    {
        public NetworkException() : base()
        {
        }

        public NetworkException(string message) : base(message)
        {
        }

        public NetworkException(string message, Exception exception) : base(message, exception)
        {
        }

        public NetworkException(int error) : this(error, string.Empty, null)
        {
        }

        public NetworkException(Enum error) : this(Convert.ToInt32(error), string.Empty, null)
        {
        }

        public NetworkException(int error, string message) : this(error, message, null)
        {
        }

        public NetworkException(Enum error, string message) : this(Convert.ToInt32(error), message, null)
        {
        }

        public NetworkException(Enum error, string message, Exception exception) : this(Convert.ToInt32(error), message, exception)
        {
        }

        public NetworkException(int error, string message, Exception exception) : base(message, exception)
        {
            HResult = error;
        }
    }
}