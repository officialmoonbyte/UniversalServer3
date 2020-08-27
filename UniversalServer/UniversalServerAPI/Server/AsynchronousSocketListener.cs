using Moonbyte.Logging;
using Moonbyte.UniversalServerAPI;
using Moonbyte.UniversalServerAPI.Client;
using Moonbyte.UniversalServerAPI.Plugin;
using Moonbyte.UniversalServerAPI.TcpServer;
using MoonbyteSettingsManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Moonbyte.UniversalServer.TcpServer
{
    public class AsynchronousSocketListener
    {

        #region Vars

        List<UniversalPlugin> serverPlugins = new List<UniversalPlugin>();

        private EventTracker eventTracker;
        IPluginLoader pluginLoader = new IPluginLoader();
        private MSMCore serverSettings = new MSMCore();
        bool pluginsLoaded = false;
        Socket serverlistener = null;
        AsynchronousWebSocketListener webServer;

        public string ServerName;
        public Logger serverPluginLogger = new Logger();
        public int Port;
        public ManualResetEvent allDone = new ManualResetEvent(false);
        public int Clients = 0;
        bool _pluginsLoaded = false;
        bool webServerEnabled = true;
        Socket Serverlistener = null;

        #endregion Vars

        #region Directories

        public string ServerDirectory;
        public string PluginDirectory;

        #endregion Directories

        #region Settings

        private int getServerPort()
        {
            string settingTitle = "ServerPort"; int defaultValue = 7876;
            if (serverSettings.CheckSetting(settingTitle))
            { return int.Parse(serverSettings.ReadSetting(settingTitle)); }
            else { serverSettings.EditSetting(settingTitle, defaultValue.ToString()); return defaultValue; }
        }

        #endregion Settings

        #region Initialization

        public AsynchronousSocketListener(string serverName, bool _webServerEnabled = true)
        {
            webServerEnabled = _webServerEnabled;
            ServerName = serverName;
            eventTracker = new EventTracker(this);
            ServerDirectory = Path.Combine(Environment.CurrentDirectory, "Servers", ServerName);
            PluginDirectory = Path.Combine(ServerDirectory, "Plugins");;

            //Creates the server directory
            try
            {
                if (!Directory.Exists(ServerDirectory)) { Directory.CreateDirectory(ServerDirectory); }
                if (!Directory.Exists(PluginDirectory)) { Directory.CreateDirectory(PluginDirectory); }
            }
            catch { }

            serverSettings.ShowLog = false;
            serverSettings.SettingsDirectory = ServerDirectory;
        }

        #endregion

        #region GetStartupDirectory

        public string StartupDirectory { get { return Environment.CurrentDirectory; } }

        #endregion GetStartupDirectory

        #region GetServerDirectory

        public string GetServerDirectory
        {
            get
            {
                string dir = Path.Combine(StartupDirectory, "Servers", this.ServerName);
                if (Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        #endregion GetServerDirectory

        #region GetPluginDirectory

        public string GetPluginDirectory
        {
            get
            { string dir = Path.Combine(GetServerDirectory, "Plugins"); return dir; }
        }

        #endregion GetPluginDirectory

        #region StartListening

        public void StartListening()
        {
            //Gets the port
            int ServerPort = this.getServerPort();
            this.Port = ServerPort;

            if (webServerEnabled)
            { webServer = new AsynchronousWebSocketListener(IPAddress.Any.ToString(), Port + 1); }

            serverPlugins = pluginLoader.LoadPlugins(GetPluginDirectory);
            pluginsLoaded = true;

            serverlistener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverlistener.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverlistener.Listen(200);
            serverlistener.BeginAccept(onSocketAccepted, null);
        }

        #endregion StartListening

        #region OnSocketAccepted

        private void onSocketAccepted(IAsyncResult result)
        {
            // This is the client socket, where you send/receive data from after accepting. Keep it in a List<Socket> collection if you need to.
            Socket client = serverlistener.EndAccept(result);

            ILogger.AddToLog("INFO", "Accepted client endpoint [" + client.RemoteEndPoint + "]");

            ClientWorkObject workObject = new ClientWorkObject(client, this);
            client.BeginReceive(workObject.buffer, 0, workObject.buffer.Length, SocketFlags.None, onDataReceived, workObject);
            serverlistener.BeginAccept(onSocketAccepted, null);
        }

        #endregion OnSocketAccepted

        #region Processing Data

        private void processDataReceived(ClientWorkObject workObject, IAsyncResult result)
        {
            string[] dataReceived = bytesToStringArray(workObject, result);

            //Get all IUniversalPackets into a list and then use Linq to sort out one that matches this one and then use JsonSerializer to serialize the object
        }

        #region Bytes to string[]

        private string[] bytesToStringArray(ClientWorkObject workObject, IAsyncResult result)
        {
            string[] returnStringArray = null;
            int bytesRead = workObject.clientSocket.EndReceive(result);

            if (bytesRead > 0)
            {
                workObject.sb.Append(Encoding.ASCII.GetString(workObject.buffer, 0, bytesRead));

                string content = workObject.sb.ToString();

                if (content.IndexOf("<EOF>") > -1)
                {
                    returnStringArray = content.Split(new string[] { "|SPLT|" },
                                    StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    workObject.clientSocket.BeginReceive(workObject.buffer, 0, workObject.buffer.Length, SocketFlags.None, onDataReceived, workObject);
                }
            }

            return returnStringArray;
        }

        #endregion Bytes to string[]

        #endregion Processing Data

        #region OnDataReceived

        private void onDataReceived(IAsyncResult result)
        {
            ClientWorkObject workObject = result.AsyncState as ClientWorkObject;

            try
            {
                processDataReceived(workObject, result);
            }
            catch (Exception e)
            {
                ILogger.LogExceptions(e);
                workObject.Dispose();
            }
        }

        #endregion OnDataReceived

        #region GetPlugin

        public UniversalPlugin GetPlugin(string pluginName)
        {
            return serverPlugins.FirstOrDefault(s => s.core.Name == pluginName);
        }

        #endregion GetPlugin

        #region GetLoadedPlugins

        public List<UniversalPlugin> GetLoadedPlugins() 
        { 
            return serverPlugins; 
        }

        #endregion GetLoadedPlugins

        #region Send

        public void Send(ClientWorkObject workObject, string data, bool useEncryption = true)
        {
            string header = useEncryption + "|";

            if (useEncryption) { data = workObject.Encryption.Encrypt(data, workObject.Encryption.GetClientPublicKey()); }

            data = header + data;

            workObject.clientSocket.Send(Encoding.ASCII.GetBytes(data));
        }

        #endregion Send

        #region ConsoleInvoke

        public void ConsolePluginInvoke(string[] args)
        {
            bool found = false; if (pluginsLoaded)
            {
                foreach (UniversalPlugin plugin in serverPlugins)
                { 
                    if (args[0].ToUpper() == plugin.core.Name.ToUpper()) 
                    {
                        found = true;
                        try
                        {
                            plugin.core.ConsoleInvoke(args, serverPluginLogger);
                        }
                        catch (Exception e)
                        {
                            ILogger.AddToLog("WARN", "Exception occured while executing a Console Plugin Command!");
                            ILogger.LogExceptions(e);
                        }
                        break; 
                    } 
                }
            }
            else { ILogger.AddToLog("INFO", "Plugins hasn't been loaded in the appdomain yet! Please load them by starting the server."); }
            
            if (!found) { ILogger.AddToLog("INFO", "Couldn't find plugin [" + args[0] + "] please make sure that plugin is installed and loaded! "); }
        }

        #endregion ConsoleInvoke

        #region InternalServerCommands

        private bool checkInternalCommands(string rawCommand, ClientWorkObject workObject)
        {
            bool returnValue = false;

            string[] args = rawCommand.Split(' ');

            if (args[0].ToUpper() == "KEY_SERVERPUBLIC")
            { Send(workObject, workObject.Encryption.GetServerPublicKey(), false); returnValue = true; }
            if (args[0].ToUpper() == "KEY_CLIENTPUBLIC")
            { workObject.Encryption.SetClientPublicKey(workObject.Encryption.Decrypt(args[1], workObject.Encryption.GetServerPrivateKey())); Send(workObject, true.ToString(), false); returnValue = true; }
            if (args[0].ToUpper() == "KEY_CLIENTPRIVATE")
            { workObject.Encryption.SetClientPrivateKey(workObject.Encryption.Decrypt(args[1], workObject.Encryption.GetServerPrivateKey())); Send(workObject, true.ToString(), false); returnValue = true; }

            return returnValue;
        }

        #region GetPublicServerKey



        #endregion GetPublicServerKey

        #endregion InternalServerCommands

        #region Dispose

        public void Dispose()
        {
            //if (Serverlistener != null)
           // { continueListening = false; }
        }

        #endregion Dispose

        #region SendCallback

        private void sendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrive the WorkObject from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);

                //Log Sending Info
            }
            catch (Exception e)
            {

            }
        }

        #endregion SendCallback
    }
}
