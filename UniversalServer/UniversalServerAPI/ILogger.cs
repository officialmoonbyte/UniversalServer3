using Moonbyte.Logging;
using System;

namespace Moonbyte.UniversalServerAPI
{
    public class Logger
    {
        public void AddToLog(string Header, string Value)
        { ILogger.AddToLog(Header, Value); }

        public void AddWhitespace()
        { ILogger.AddWhitespace(); }

        public void LogExceptions(Exception e)
        { ILogger.LogExceptions(e); }
    }
}
