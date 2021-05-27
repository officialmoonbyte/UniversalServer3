using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using System.Collections.Generic;
using UniversalServer.Core.Globalization;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class StartServer : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("startserver");
            activeStrings.Add("startservers");
            activeStrings.Add("start");

            return activeStrings;
        }

        public void RunCommand(string[] args)
        {
            var serverName = args[1];

            bool found = false; foreach (AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == serverName)
                { listener.StartListening(); ILogger.AddToLog("INFO", serverName + Messages.ConsoleCommands.ServerStartedNotification[1] + listener.Port); found = true; break; }
            }
            if (!found) { ILogger.AddToLog(Messages.ConsoleCommands.CantFindServer[0], Messages.ConsoleCommands.CantFindServer[1] + serverName + Messages.ConsoleCommands.CantFindServer[2]); }
        }
    }
}
