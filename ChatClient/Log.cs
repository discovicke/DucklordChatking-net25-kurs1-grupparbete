using System;
using System.IO;

namespace ChatClient
{
    public enum LogLevel
    {
        Info, // Black
        Success, // Green
        Error // Red
    }

    public class Log
    {
        private static readonly string LogFilePath = "log.txt";
        private static readonly object lockObject = new object();

        public static void Write(string message, LogLevel level = LogLevel.Info)
        {
            string timestamp = DateTime.Now.ToString("[dd/MM/yy HH:mm:ss]");
            string colorCode = GetColorCode(level);
            string logEntry = $"{timestamp} {colorCode}\t{message}";

            lock (lockObject)
            {
                try
                {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log: {ex.Message}");
                }
            }
        }

        private static string GetColorCode(LogLevel level)
        {
            return level switch
            {
                LogLevel.Success => "[SUCCESS]",
                LogLevel.Error => "[ERROR]",
                LogLevel.Info => "[INFO]",
                _ => "[INFO]"
            };
        }

        public static void Info(string message) => Write(message, LogLevel.Info);
        public static void Success(string message) => Write(message, LogLevel.Success);
        public static void Error(string message) => Write(message, LogLevel.Error);
    }
}