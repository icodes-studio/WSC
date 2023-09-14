namespace WSC
{
    public static class NetworkTypes
    {
        public const int RECOVERY = 10;
    }

    public enum NetworkError
    {
        Success = 0,
        CommandNotFound = 9000,
        InternalError = 9001,
        SendFailed = 9002,
        InvalidData = 9003,
        Closed = 9004,
        Network = 9005,
        AlreadyExists = 9006,
        InvalidTicket = 9007,
        NoData = 9008,
        Exception = 9009,
        Unknown = 9999,
    }
}