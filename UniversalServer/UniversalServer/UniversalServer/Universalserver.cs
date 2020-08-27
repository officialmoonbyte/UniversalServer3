﻿using Moonbyte.Logging;
using Moonbyte.UniversalServer.TcpServer;
using MoonbyteSettingsManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniversalServer.Commandline;

namespace UniversalServer
{
    public static class Universalserver
    {

        #region Vars

        public static MSMCore SettingsManager = new MSMCore();
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

        //CreateServer
        //List
        //Help

        public static void ProcessCommand(string[] Args)
        {
            string Command = Args[0].ToUpper();

            try
            {
                // CreateServer, Create [ServerName]
                if (Command == "CREATESERVER" || Command == "CREATE")
                { ConsoleCommands.CreateServer(Args[1]); }
                //ListServers, ListServer, List
                if (Command == "LISTSERVERS" || Command == "LISTSERVER" || Command == "LIST")
                { ConsoleCommands.ListServer(); }
                //StartServer, StartServers, Start
                if (Command == "STARTSERVER" || Command == "STARTSERVERS" || Command == "START")
                { ConsoleCommands.StartServer(Args[1]); }
                //StopServer, Dispose, Stop, StopServers
                if (Command == "STOPSERVER" || Command=="STOPSERVERS" || Command == "DISPOSE" || Command == "STOP")
                { ConsoleCommands.StopServer(Args[1]); }
                if (Command == "INVOKE" || Command == "INVOKECOMMAND" || Command == "INVOKECONSOLECOMMAND")
                {
                    List<string> tempArray = new List<string>(Args);
                    tempArray.RemoveAt(0); tempArray.RemoveAt(0);

                    ConsoleCommands.InvokeConsoleCommand(Args[1], tempArray.ToArray());
                }
            }
            catch (Exception e)
            { ILogger.LogExceptions(e); }

        }

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
