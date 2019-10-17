using Moonbyte.UniversalServer.TcpServer;
using System.Collections.Generic;
using UniversalServer.Commandline;

namespace UniversalServer
{
    public static class Universalserver
    {

        #region Vars

        public static universalServerCommandLine Commandline = new universalServerCommandLine();
        public static universalServerSettingsManager SettingsManager = new universalServerSettingsManager();
        public static List<AsynchronousSocketListener> TcpServers = new List<AsynchronousSocketListener>();

        #endregion Vars

        #region Close

        public static void Close()
        {
            SettingsManager.SaveCurrentSettings();

            foreach (AsynchronousSocketListener tcpServer in TcpServers)
            { tcpServer.Dispose(); }
        }

        #endregion Close

    }
}
