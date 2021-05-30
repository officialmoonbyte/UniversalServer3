using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Model;
using Moonbyte.UniversalServer.Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public (string, string) GetHelpCommandLog()
            => ("Createserver [ServerName]", "Creates a new server in Universal Server.");

        public string GetName()
            => "CreateServer";

        public void RunCommand(string[] args)
        {
            string serverName = args[1];

            var universalServer = Universalserver.TcpServers.FirstOrDefault(x => Utility.EqualsIgnoreCase(x.ServerName, serverName));
            if (universalServer != null)
            {
                ILogger.AddToLog(ILogger.Levels.INFO, "Error while trying to create a server! Server already exists!");
                return;
            }

            //Initializes the server and creates the server
            AsynchronousSocketListener serverListener = new AsynchronousSocketListener(serverName);
            Universalserver.TcpServers.Add(serverListener);

            ILogger.AddToLog(ILogger.Levels.INFO, "Created " + serverName + ".");
            ILogger.AddToLog(ILogger.Levels.INFO, "Great! " + serverName + " has been made! To start " + serverName + " just type [start " + serverName + "]");
            ILogger.AddToLog(ILogger.Levels.INFO, $"You can also install plugins to your new server!, to install a plugin download run the upd command or manually install the plugin in this directory {Environment.NewLine}{serverListener.PluginDirectory}");
        }
    }
}
