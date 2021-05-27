using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using System.Collections.Generic;
using UniversalServer.Core.Globalization;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class StopServer : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("stopserver");
            activeStrings.Add("stopservers");
            activeStrings.Add("stop");

            return activeStrings;
        }

        public void RunCommand(string[] args)
        {
            var serverName = args[1];

            bool found = false; foreach (AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == serverName)
                { listener.Dispose(); ILogger.AddToLog(Messages.ConsoleCommands.DisposeCallNotification[0], Messages.ConsoleCommands.DisposeCallNotification[1] + serverName); found = true; }
            }
            if (!found) { ILogger.AddToLog(Messages.ConsoleCommands.CantFindServer[0], Messages.ConsoleCommands.CantFindServer[1] + serverName + Messages.ConsoleCommands.CantFindServer[2]); }
        }
    }
}
