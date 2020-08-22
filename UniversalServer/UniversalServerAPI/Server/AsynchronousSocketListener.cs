﻿using Moonbyte.Logging;
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
        bool PluginsLoaded = false;
        bool _WebServer = true;
        Socket Serverlistener = null;

        #endregion Vars

        #region Directories

        public string ServerDirectory;
        public string PluginDirectory;

        #endregion Directories

        #region Settings

        private int GetServerPort()
        {
            string settingTitle = "ServerPort"; int defaultValue = 7876;
            if (serverSettings.CheckSetting(settingTitle))
            { return int.Parse(serverSettings.ReadSetting(settingTitle)); }
            else { serverSettings.EditSetting(settingTitle, defaultValue.ToString()); return defaultValue; }
        }

        #endregion Settings

        #region Initialization

        public AsynchronousSocketListener(string serverName, string ServerDirectory, bool WebServer = true)
        {
            _WebServer = WebServer;
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
            int ServerPort = this.GetServerPort();
            this.Port = ServerPort;

            if (_WebServer)
            { webServer = new AsynchronousWebSocketListener(IPAddress.Any.ToString(), Port + 1); }

            serverPlugins = pluginLoader.LoadPlugins(GetPluginDirectory);
            PluginsLoaded = true;

            serverlistener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverlistener.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverlistener.Listen(200);
            serverlistener.BeginAccept(OnSocketAccepted, null);
        }

        #endregion StartListening

        #region OnSocketAccepted

        private void OnSocketAccepted(IAsyncResult result)
        {
            // This is the client socket, where you send/receive data from after accepting. Keep it in a List<Socket> collection if you need to.
            Socket client = serverlistener.EndAccept(result);

            ILogger.AddToLog("INFO", "Accepted client endpoint [" + client.RemoteEndPoint + "]");

            ClientWorkObject workObject = new ClientWorkObject(client, this);
            client.BeginReceive(workObject.buffer, 0, workObject.buffer.Length, SocketFlags.None, OnDataReceived, workObject);
            serverlistener.BeginAccept(OnSocketAccepted, null);
        }

        #endregion OnSocketAccepted

        #region OnDataReceived

        private void OnDataReceived(IAsyncResult result)
        {
            ClientWorkObject workObject = result.AsyncState as ClientWorkObject;

            try
            {
                int bytesRead = workObject.clientSocket.EndReceive(result);

                //Handles received data in buffer
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far
                    workObject.sb.Append(Encoding.ASCII.GetString(workObject.buffer, 0, bytesRead));

                    string content = workObject.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        workObject.sb = new StringBuilder();
                        workObject.buffer = new byte[ClientWorkObject.BufferSize];

                        content = content.Replace("<EOF>", "");

                        string[] contentsplit = content.Split('.');

                    }
                    else
                    {
                        workObject.clientSocket.BeginReceive(workObject.buffer, 0, workObject.buffer.Length, SocketFlags.None, OnDataReceived, workObject);
                    }
                }
            }
            catch (Exception e)
            {
                ILogger.LogExceptions(e);
                workObject.Dispose();
            }
        }

        #endregion OnDataReceived

        #region GetPlugin

        public UniversalPlugin getPlugin(string PluginName)
        {
            return serverPlugins.FirstOrDefault(s => s.core.Name == PluginName);
        }

        #endregion GetPlugin

        #region GetLoadedPlugins

        public List<UniversalPlugin> GetLoadedPlugins() 
        { 
            return serverPlugins; 
        }

        #endregion GetLoadedPlugins

        #region Send

        public void Send(ClientWorkObject WorkObject, string Data, bool UseEncryption = true)
        {
            string header = UseEncryption + "|";

            if (UseEncryption) { Data = WorkObject.Encryption.Encrypt(Data, WorkObject.Encryption.GetClientPublicKey()); }

            Data = header + Data;

            WorkObject.clientSocket.Send(Encoding.ASCII.GetBytes(Data));
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

        private bool CheckInternalCommands(string RawCommand, ClientWorkObject workObject)
        {
            bool returnValue = false;

            string[] args = RawCommand.Split(' ');

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

        private void SendCallback(IAsyncResult ar)
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