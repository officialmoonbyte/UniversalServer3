using System;

namespace Moonbyte.UniversalServerAPI.Plugin.module
{
    /// <summary>
    /// Used for plugins to log events.
    /// </summary>
    public class ILogger
    {
        public void AddToLog(string Header, string Value)
        { Logging.ILogger.AddToLog(Header, Value); }

        public void AddWhitespace()
        { Logging.ILogger.AddWhitespace(); }

        public void LogExceptions(Exception e)
        { Logging.ILogger.LogExceptions(e); }
    }
}
