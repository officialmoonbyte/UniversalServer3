using Moonbyte.UniversalServer.Core.Logging;
using System.Collections.Generic;
using System.Linq;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class ListServer : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("listservers");
            activeStrings.Add("listserver");
            activeStrings.Add("list");

            return activeStrings;
        }

        public (string, string) GetHelpCommandLog()
            => ("List", "Displays all of the servers information and status.");

        public string GetName()
            => "ListServer";
        public void RunCommand(string[] args)
        {
            Universalserver.TcpServers.OrderBy(x => x.ServerName).ToList().ForEach(x =>
            {
                string onlineText = "Online!";

                if (!x.IsListening) onlineText = "Offline!";

                ILogger.AddToLog("INFO", $"{x.ServerName} : Status : {onlineText}  Clients Connected : {x.Clients}");
            });
        }
    }
}
