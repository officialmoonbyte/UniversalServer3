using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Model;
using System.Collections.Generic;
using System.Linq;
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

        public (string, string) GetHelpCommandLog()
            => ("Stopserver [ServerName]", "Stops the selected server.");

        public string GetName()
            => "StopServer";

        public void RunCommand(string[] args)
        {
            var serverName = args[1];

            var universalServer = Universalserver.TcpServers.FirstOrDefault(x => Utility.EqualsIgnoreCase(x.ServerName, serverName));
            if (universalServer == null)
            {
                ILogger.AddToLog(Messages.ConsoleCommands.CantFindServer[0], Messages.ConsoleCommands.CantFindServer[1] + serverName + Messages.ConsoleCommands.CantFindServer[2]);
            }

            universalServer.Dispose();
            ILogger.AddToLog(Messages.ConsoleCommands.DisposeCallNotification[0], Messages.ConsoleCommands.DisposeCallNotification[1] + serverName);
        }
    }
}
