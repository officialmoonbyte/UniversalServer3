using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using System.Collections.Generic;
using UniversalServer.Interfaces;
using UniversalServer.Core.Globalization;

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

        public void RunCommand(string[] args)
        {
            List<string> tempArray = new List<string>(args);
            tempArray.RemoveAt(0); tempArray.RemoveAt(0);

            var serverName = args[1];


            bool found = false; foreach (AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == serverName)
                {
                    found = true;

                    listener.ConsolePluginInvoke(tempArray.ToArray());

                    break;
                }
            }
            if (!found) { ILogger.AddToLog(Messages.ConsoleCommands.CouldntfindServer[0], Messages.ConsoleCommands.CouldntfindServer[1] + serverName + Messages.ConsoleCommands.CouldntfindServer[2]); }
        }
    }
}
