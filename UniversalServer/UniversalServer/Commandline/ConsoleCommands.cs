using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using UniversalServer.Core.Globalization;

namespace UniversalServer.Commandline
{
    public class ConsoleCommands
    {
        public static void CreateServer(string ServerName)
        {
            //Initializes the server
            AsynchronousSocketListener serverListener = new AsynchronousSocketListener(ServerName);
            Universalserver.TcpServers.Add(serverListener);
            ILogger.AddToLog("INFO", "Created " + ServerName + ".");
            ILogger.AddToLog("INFO", "Great! " + ServerName + " has been made! To start " + ServerName + " just type [start " + ServerName + "]");
            ILogger.AddToLog("INFO", "You can also install plugins to your new server!, to install a plugin download one online and drag and drop the .dll file into " + serverListener.PluginDirectory);
        }

        public static void ListServer()
        {
            foreach(AsynchronousSocketListener socketListener in Universalserver.TcpServers)
            { ILogger.AddToLog("INFO", socketListener.ServerName + " : Online : , Clients Connected : " + socketListener.Clients); }

            ILogger.AddWhitespace();
        }

        public static void StartServer(string ServerName)
        {
            bool found = false; foreach(AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == ServerName)
                { listener.StartListening(); ILogger.AddToLog("INFO", ServerName + Messages.ConsoleCommands.ServerStartedNotification[1] + listener.Port); found = true; break; }
            }
            if (!found) { ILogger.AddToLog(Messages.ConsoleCommands.CantFindServer[0], Messages.ConsoleCommands.CantFindServer[1] + ServerName + Messages.ConsoleCommands.CantFindServer[2]); }
        }

        public static void InvokeConsoleCommand(string ServerName, string[] Args)
        {
            bool found = false; foreach(AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == ServerName)
                { found = true;

                    listener.ConsolePluginInvoke(Args);

                break; }
            } if (!found) { ILogger.AddToLog(Messages.ConsoleCommands.CouldntfindServer[0], Messages.ConsoleCommands.CouldntfindServer[1] + ServerName + Messages.ConsoleCommands.CouldntfindServer[2]); }
        }

        public static void StopServer(string ServerName)
        {
            bool found = false; foreach (AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == ServerName)
                { listener.Dispose(); ILogger.AddToLog(Messages.ConsoleCommands.DisposeCallNotification[0], Messages.ConsoleCommands.DisposeCallNotification[1] + ServerName); found = true; }
            }
            if (!found) { ILogger.AddToLog(Messages.ConsoleCommands.CantFindServer[0], Messages.ConsoleCommands.CantFindServer[1] + ServerName + Messages.ConsoleCommands.CantFindServer[2]); }
        }
    }
}
