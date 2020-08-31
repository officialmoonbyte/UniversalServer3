using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalServer.Core.Globalization
{
    public static class Messages
    {

        #region ConsoleCommands

        public static class ConsoleCommands
        {
            #region Create Server

            public static string[] CreatedServerNotification = new string[] { "INFO", "Created ", "." }; //Created [ServerName].
            public static string[] StartServerTutorial = new string[] { "INFO", "Great! ", " has been made! To start ", " just type [start ", "]" }; //Great! [ServerName] has been made! To start [ServerName] just type [start [ServerName]]
            public static string[] PluginTutorial = new string[] { "INFO", "You can also install plugins to your new server!, to install a plugin download one online and drag and drop the .dll file into " }; //You can also install plugins to your new server!, to install a plugin download one online and drag and drop the.dll file into [PluginDirectory]

            #endregion Create Server

            #region List Server

            public static string[] ListServer = new string[] { "INFO", " : Online : , Clients Connected : " }; // [ServerName] : Online : , Clients Connected : 

            #endregion List Server

            #region Start Server

            public static string[] ServerStartedNotification = new string[] { "INFO", " has started listening on port " }; // [ServerName] has started listening on port [Port]
            public static string[] CantFindServer = new string[] { "INFO", "Couldn't find server with the title '", "'"}; //Couldn't find server with the title '[ServerName]'

            #endregion Start Server

            #region InvokeConsoleCommand

            public static string[] CouldntfindServer = new string[] { "INFO", "Couldn't find the server with the title '", "'" }; //Couldn't find the server with the title 'ServerName'

            #endregion InvokeConsoleCommand

            #region StopServer

            public static string[] DisposeCallNotification = new string[] { "INFO", "Dispose called on " }; //Dispose called on [ServerName]

            #endregion StopServer

        }

        #endregion ConsoleCommands

        public static string PostStatusCodeRequiredErrorMessage = "Unauthorized Error : Packet status code must be POST to invoke this command!";
    }
}
