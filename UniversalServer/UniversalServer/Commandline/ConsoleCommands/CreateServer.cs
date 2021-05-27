using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using System.Collections.Generic;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class CreateServer : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("createserver");
            activeStrings.Add("create");

            return activeStrings;
        }

        public void RunCommand(string[] args)
        {
            string serverName = args[1];

            //Initializes the server and creates the server
            AsynchronousSocketListener serverListener = new AsynchronousSocketListener(serverName);
            Universalserver.TcpServers.Add(serverListener);

            ILogger.AddToLog("INFO", "Created " + serverName + ".");
            ILogger.AddToLog("INFO", "Great! " + serverName + " has been made! To start " + serverName + " just type [start " + serverName + "]");
            ILogger.AddToLog("INFO", "You can also install plugins to your new server!, to install a plugin download one online and drag and drop the .dll file into " + serverListener.PluginDirectory);
        }
    }
}
