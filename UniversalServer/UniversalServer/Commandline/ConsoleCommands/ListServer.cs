using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using System.Collections.Generic;
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

        public void RunCommand(string[] args)
        {
            foreach (AsynchronousSocketListener socketListener in Universalserver.TcpServers)
            { ILogger.AddToLog("INFO", socketListener.ServerName + " : Online : , Clients Connected : " + socketListener.Clients); }

            ILogger.AddWhitespace();
        }
    }
}
