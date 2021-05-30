using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Model;
using Moonbyte.UniversalServer.Core.Server;
using System.Collections.Generic;
using System.Linq;
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

        public (string, string) GetHelpCommandLog()
            => ("Startserver [ServerName]", "Starts the selected server.");

        public string GetName()
            => "StartServer";
        public void RunCommand(string[] args)
        {
            var serverName = args[1];
            var universalServer = Universalserver.TcpServers.FirstOrDefault(x => Utility.EqualsIgnoreCase(x.ServerName, serverName));
            if (universalServer == null)
            {
                ILogger.AddToLog(Messages.ConsoleCommands.CantFindServer[0], Messages.ConsoleCommands.CantFindServer[1] + serverName + Messages.ConsoleCommands.CantFindServer[2]);
                return;
            }

            universalServer.StartListening();
            ILogger.AddToLog("INFO", serverName + Messages.ConsoleCommands.ServerStartedNotification[1] + universalServer.Port);
        }
    }
}
