using System;
using Moonbyte.UniversalServer.Core.Logging;

namespace Moonbyte.UniversalServer.Plugin.Module
{
    /// <summary>
    /// Used for plugins to log events.
    /// </summary>
    public class Logger
    {
        public void AddToLog(ILogger.Levels level, string value)
        { ILogger.AddToLog(level, value); }

        public void AddWhitespace()
        { ILogger.AddWhitespace(); }

        public void LogExceptions(Exception e)
        { ILogger.LogExceptions(e); }
    }
}
