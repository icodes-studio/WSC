using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace WSC
{
    public enum LogLevel
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4,
        Message = int.MaxValue
    }

    public static class Log
    {
        private const int MaxFileSize = 20480000;

        private static string name = string.Empty;
        private static string location = string.Empty;
        private static object fileLock = new object();
        private static Action<LogLevel, string> callback = null;
        private static FileStream file = null;
        private static long index = 0;

        public static LogLevel Level
        {
            get; set;
        } = LogLevel.Debug;

        public static void Initialize(string path, string name, LogLevel level = LogLevel.Debug, Action<LogLevel, string> callback = null)
        {
            Log.name = name;
            Log.location = path + name;
            Log.Level = level;
            Log.callback = callback;

            if (string.IsNullOrEmpty(Log.location) == false)
                if (Directory.Exists(Log.location) == false)
                    Directory.CreateDirectory(Log.location);
        }

        public static void Debug(string message) =>
            Write(LogLevel.Debug, message);

        public static void Debug(object message) =>
            Write(LogLevel.Debug, message?.ToString());

        public static void Debug(string format, params object[] args) =>
            Write(LogLevel.Debug, string.Format(format, args));

        public static void Info(string message) =>
            Write(LogLevel.Information, message);

        public static void Info(object message) =>
            Write(LogLevel.Information, message?.ToString());

        public static void Info(string format, params object[] args) =>
            Write(LogLevel.Information, string.Format(format, args));

        public static void Warning(string message) =>
            Write(LogLevel.Warning, message);

        public static void Warning(object message) =>
            Write(LogLevel.Warning, message?.ToString());

        public static void Warning(string format, params object[] args) =>
            Write(LogLevel.Warning, string.Format(format, args));

        public static void Error(string message) =>
            Write(LogLevel.Error, message);

        public static void Error(object message) =>
            Write(LogLevel.Error, message?.ToString());

        public static void Error(string format, params object[] args) =>
            Write(LogLevel.Error, string.Format(format, args));

        public static void Fatal(string message) =>
            Write(LogLevel.Fatal, $"{message}\n{new StackTrace(1, true)}");

        public static void Fatal(object message) =>
            Write(LogLevel.Fatal, $"{message}\n{new StackTrace(1, true)}");

        public static void Fatal(string format, params object[] args) =>
            Write(LogLevel.Fatal, $"{string.Format(format, args)}\n{new StackTrace(1, true)}");

        public static void Message(string message) =>
            Write(LogLevel.Message, $"{message}");

        public static void Message(object message) =>
            Write(LogLevel.Message, $"{message}");

        public static void Message(string format, params object[] args) =>
            Write(LogLevel.Message, $"{string.Format(format, args)}");

#if NET5_0_OR_GREATER
        public static void Assert(bool condition, [CallerArgumentExpression(nameof(condition))] string message = null)
#else
        public static void Assert(bool condition, string message = "false")
#endif
        {
            if (condition == false)
            {
                Write(LogLevel.Fatal, $"Assert({message})\n{new StackTrace(1, true)}");
                Trace.Assert(condition);
            }
        }

        public static void Write(LogLevel level, string message)
        {
            if (Log.Level <= level)
            {
                var method = new StackTrace(true).GetFrame(2)?.GetMethod();
                var messages = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [{level}] [{method?.ReflectedType?.Name}.{method?.Name}] {message}";

                Write(messages);

                callback?.Invoke(level, messages);
            }
        }

        private static void Write(string message)
        {
            if (string.IsNullOrEmpty(location) == false)
            {
                lock (fileLock)
                {
                    var fileName = $@"{location}\{name}-{DateTime.Now.ToString("yyyyMMdd")}";
                    var fullPath = fileName + ".log";
                    var bytes = Encoding.UTF8.GetBytes(message);
                    var line = Encoding.ASCII.GetBytes(Environment.NewLine);

                    if (file == null)
                        file = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Read);

                    file.Write(bytes, 0, bytes.Length);
                    file.Write(line, 0, line.Length);
                    file.Flush();

                    if (file.Length > MaxFileSize)
                    {
                        file.Dispose();
                        file = null;
                        File.Move(fullPath, $"{fileName}#{index++}.log");
                    }
                }
            }
        }
    }
}