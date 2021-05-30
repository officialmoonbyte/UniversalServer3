using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Model;
using System.Collections.Generic;
using System.Linq;
using UniversalServer.Core.Globalization;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class InvokeConsoleCommand : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("invoke");
            activeStrings.Add("invokecommand");
            activeStrings.Add("invokeconsolecommand");

            return activeStrings;
        }

        public (string, string) GetHelpCommandLog()
            => ("Invoke [ServerName]", "Invokes a command on the server.");

        public string GetName()
            => "InvokeConsoleCommand";

        public void RunCommand(string[] args)
        {
            List<string> tempArray = new List<string>(args);
            tempArray.RemoveAt(0); tempArray.RemoveAt(0);

            var serverName = args[1];

            var universalServer = Universalserver.TcpServers.FirstOrDefault(x => Utility.EqualsIgnoreCase(x.ServerName, serverName));
            if (universalServer != null)
            {
                universalServer.ConsolePluginInvoke(tempArray.ToArray());
                return;
            }

            ILogger.AddToLog(ILogger.Levels.FATAL, Messages.ConsoleCommands.CouldntfindServer[1] + serverName + Messages.ConsoleCommands.CouldntfindServer[2]); 
        }
    }
}
