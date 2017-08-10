using System;

namespace HelpdeskKit.Logger
{
    public interface ILogger
    {
        Type ClassType { get; }
        void Log(string log);
        event OnNewLogHandler OnNewLog;
    }
}