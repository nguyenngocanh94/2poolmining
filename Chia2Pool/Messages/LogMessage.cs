using System;

namespace Chia2Pool.Messages
{
    public class LogMessage
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        
        public LogLevel LogLevel { get; set; }
    }

    public enum LogLevel
    {
        INFO,
        WARN,
        FATAL,
    }
}