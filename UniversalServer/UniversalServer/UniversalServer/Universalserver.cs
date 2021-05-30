using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Server;
using MoonbyteSettingsManager;
using System;
using System.Collections.Generic;
using System.IO;
using UniversalServer.Commandline;
using UniversalServer.Handlers;

namespace UniversalServer
{
    public static class Universalserver
    {

        #region Vars

        public static MSMCore SettingsManager = new MSMCore();
        public static CommandHandler CommandHandler = new CommandHandler();
        public static List<AsynchronousSocketListener> TcpServers = new List<AsynchronousSocketListener>();

        #endregion Vars

        #region Directories

        public static string ServerDirectories;

        #endregion Directories

        #region InitializeServers

        private static void InitializeServers()
        {
            foreach(string directory in Directory.GetDirectories(ServerDirectories))
            {
                string serverName = new DirectoryInfo(directory).Name;
                AsynchronousSocketListener socketListener = new AsynchronousSocketListener(serverName);
                TcpServers.Add(socketListener);
            }
        }

        #endregion InitializeServers

        #region InitializeCommandlineThread

        private static void InitializeCommandlineThread()
        { CommandLine commandLine = new CommandLine(); }

        #endregion InitializeCommandlineThread

        #region Initialize

        public static void InitializeUniversalServer()
        {
            //Initialize Server Directories
            ServerDirectories = Path.Combine(Environment.CurrentDirectory, "Servers");

            //Checks if the directories exist
            if (!Directory.Exists(ServerDirectories)) Directory.CreateDirectory(ServerDirectories);

            //Initializes SettingsManager
            SettingsManager.ShowLog = false;
            SettingsManager.SettingsDirectory = Environment.CurrentDirectory;

            //Initialize Components
            InitializeServers();
            InitializeCommandlineThread();
        }

        #endregion Initialize

        #region ProcessCommand

        public static void ProcessCommand(string[] Args)
            => CommandHandler.HandleCommand(Args);

        #endregion ProcessCommand

        #region Close

        public static void Close()
        {
            SettingsManager.SaveSettings();

            foreach (AsynchronousSocketListener tcpServer in TcpServers)
            { tcpServer.Dispose(); }
        }

        #endregion Close

    }
}
