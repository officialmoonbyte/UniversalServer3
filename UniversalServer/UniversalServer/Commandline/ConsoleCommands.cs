using Moonbyte.Logging;
using Moonbyte.UniversalServer.TcpServer;

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
            ILogger.AddToLog("INFO", "To start the seBrver, please type the command [Start " + ServerName + "]");
            ILogger.AddToLog("INFO", "To add plugins to the server, drag and drop the plugins to " + serverListener.PluginDirectory);
        }

        public static void ListServer()
        {
            foreach(AsynchronousSocketListener socketListener in Universalserver.TcpServers)
            { ILogger.AddToLog("INFO", socketListener.ServerName + " : " + "Online : " + ", Clients Connected : " + socketListener.Clients); }

            ILogger.AddWhitespace();
        }

        public static void StartServer(string ServerName)
        {
            bool found = false; foreach(AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == ServerName)
                { listener.StartListening(); ILogger.AddToLog("INFO", ServerName + " has started listening on port " + listener.Port); found = true; break; }
            }
            if (!found) { ILogger.AddToLog("INFO", "Couldn't find server with the title '" + ServerName + "'"); }
        }

        public static void InvokeConsoleCommand(string ServerName, string[] Args)
        {
            bool found = false; foreach(AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == ServerName)
                { found = true;

                    listener.ConsolePluginInvoke(Args);

                break; }
            } if (!found) { ILogger.AddToLog("INFO", "Couldn't find server with the title '" + ServerName + "'"); }
        }

        public static void StopServer(string ServerName)
        {
            bool found = false; foreach (AsynchronousSocketListener listener in Universalserver.TcpServers)
            {
                if (listener.ServerName == ServerName)
                { listener.Dispose(); ILogger.AddToLog("INFO", "Dispose called on " + ServerName); found = true; }
            }
            if (!found) { ILogger.AddToLog("INFO", "Couldn't find server with the title '" + ServerName + "'"); }
        }
    }
}
