using System;
using Chia2Pool.Messages;
using Chia2Pool.Models;

namespace Chia2Pool.Common
{
    public class Logger
    {
        public static Func<log4net.ILog> GetLog;

        public static LogMessage Warn(string mess)
        {
            return new()
            {
                Date = DateTime.Now,
                Message = mess,
                LogLevel = LogLevel.WARN
            };
        }
        
        public static LogMessage Info(string mess)
        {
            return new()
            {
                Date = DateTime.Now,
                Message = mess,
                LogLevel = LogLevel.INFO
            };
        }
        
        public static LogMessage Fatal(string mess)
        {
            return new()
            {
                Date = DateTime.Now,
                Message = mess,
                LogLevel = LogLevel.FATAL
            };
        }
        
        public static LogEntry W(string mess)
        {
            return new LogEntry()
            {
                DateTime = DateTime.Now,
                Message = mess,
                Level = "WARN"
            };
        }
        
        public static LogEntry I(string mess)
        {
            return new()
            {
                DateTime = DateTime.Now,
                Message = mess,
                Level = "INFO"
            };
        }
        
        public static LogEntry F(string mess)
        {
            return new LogEntry()
            {
                DateTime = DateTime.Now,
                Message = mess,
                Level = "FATAL"
            };
        }
    }
}