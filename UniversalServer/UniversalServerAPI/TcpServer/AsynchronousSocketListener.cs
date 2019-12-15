﻿using Moonbyte.Logging;
using Moonbyte.UniversalServerAPI;
using MoonbyteSettingsManager;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UniversalServer.Security;

namespace Moonbyte.UniversalServer.TcpServer
{
    public class AsynchronousSocketListener
    {

        #region Vars

        public string ServerName;
        public int Port;
        private MSMCore ServerSettings = new MSMCore();
        public ManualResetEvent allDone = new ManualResetEvent(false);
        public int Clients = 0;
        bool isListening = false;
        bool continueListening = false;
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
            if (ServerSettings.CheckSetting(settingTitle))
            { return int.Parse(ServerSettings.ReadSetting(settingTitle)); }
            else { ServerSettings.EditSetting(settingTitle, defaultValue.ToString()); return defaultValue; }
        }

        #endregion Settings

        #region Initialization

        public AsynchronousSocketListener(string serverName, string ServerDirectory)
        {
            ServerName = serverName;
            ServerDirectory = Path.Combine(Environment.CurrentDirectory, "Servers", ServerName);
            PluginDirectory = Path.Combine(ServerDirectory, "Plugins");;

            //Creates the server directory
            try
            {
                if (!Directory.Exists(ServerDirectory)) { Directory.CreateDirectory(ServerDirectory); }
                if (!Directory.Exists(PluginDirectory)) { Directory.CreateDirectory(PluginDirectory); }
            }
            catch { }

            ServerSettings.SettingsDirectory = ServerDirectory;
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
                string dir = Path.Combine(StartupDirectory, this.ServerName);
                if (Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        #endregion GetServerDirectory

        #region IsListening

        public bool IsListening()
        { return isListening; }

        #endregion IsListening

        #region StartListening

        public void StartListening()
        {
            //Gets the port
            int ServerPort = this.GetServerPort();
            this.Port = ServerPort;

            Serverlistener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Serverlistener.Bind(new IPEndPoint(IPAddress.Any, Port));
            Serverlistener.Listen(200);
            Serverlistener.BeginAccept(OnSocketAccepted, null);
        }

        #endregion StartListening

        #region OnSocketAccepted

        private void OnSocketAccepted(IAsyncResult result)
        {
            // This is the client socket, where you send/receive data from after accepting. Keep it in a List<Socket> collection if you need to.
            Socket client = Serverlistener.EndAccept(result);

            ClientWorkObject workObject = new ClientWorkObject(client, this);
            client.BeginReceive(workObject.buffer, 0, workObject.buffer.Length, SocketFlags.None, OnDataReceived, workObject);
            Serverlistener.BeginAccept(OnSocketAccepted, null);
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

                        Console.WriteLine(content);
                        CheckInternalCommands(content, workObject);

                        //Starts a new async receive on the client
                        workObject.clientSocket.BeginReceive(workObject.buffer, 0, workObject.buffer.Length, SocketFlags.None, OnDataReceived, workObject);
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
            }
        }

        #endregion OnDataReceived

        #region AcceptCallback

        public void AcceptCallBack(IAsyncResult ar)
        {
            //Signal the mainthread to continue
            allDone.Set();

            //Get both sockets from the AsyncResult
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //Create the state object
            ClientWorkObject state = new ClientWorkObject(handler, this);
            handler.BeginReceive(state.buffer, 0, ClientWorkObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        #endregion AcceptCallback

        #region ReadCallback

        public void ReadCallback(IAsyncResult ar)
        {
            string content = String.Empty;

            // Get the work object from the async result.
            // Then gets the handler socket from the work object.
            ClientWorkObject workObject = (ClientWorkObject)ar.AsyncState;
            Socket handler = workObject.clientSocket;

            try
            {
                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //Download more data from the client
                    workObject.sb.Append(Encoding.ASCII.GetString(workObject.buffer, 0, bytesRead));

                    // Check for end-of-file tag, if its not there then it reads more data.
                    content = workObject.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        Console.WriteLine(content);
                        //Command Handler
                        CheckInternalCommands(content, workObject);
                        //UserLog
                    }
                    else
                    {
                        //Not all data received, get more
                        workObject.buffer = new byte[ClientWorkObject.BufferSize];
                        handler.BeginReceive(workObject.buffer, 0, ClientWorkObject.BufferSize, 0, new AsyncCallback(ReadCallback), workObject);
                    }
                }
            }
            catch (Exception e)
            {
                ILogger.LogExceptions(e);
                ILogger.AddToLog("INFO", "Client disconnected!");
            }
            finally
            {
                if (workObject != null && handler != null)
                {
                    workObject.buffer = new byte[ClientWorkObject.BufferSize];
                    handler.BeginReceive(workObject.buffer, 0, ClientWorkObject.BufferSize, 0, new AsyncCallback(ReadCallback), workObject);
                }
            }
        }

        #endregion ReadCallback

        #region Send

        public void Send(ClientWorkObject WorkObject, string Data)
        {
            WorkObject.clientSocket.Send(Encoding.ASCII.GetBytes(Data));
        }

        #endregion Send

        #region InternalServerCommands

        private bool CheckInternalCommands(string RawCommand, ClientWorkObject workObject)
        {
            bool returnValue = false;

            string[] headerSplit = RawCommand.Split('|');
            if (headerSplit[0].ToUpper() == "KEY_SERVERPUBLIC")
            { Send(workObject, workObject.Encryption.GetServerPublicKey()); }
            if (headerSplit[0].ToUpper() == "KEY_CLIENTPUBLIC")
            { workObject.Encryption.SetClientPublicKey(workObject.Encryption.Decrypt(headerSplit[1], workObject.Encryption.GetServerPrivateKey())); Send(workObject, true.ToString()); }
            if (headerSplit[0].ToUpper() == "KEY_CLIENTPRIVATE")
            { workObject.Encryption.SetClientPrivateKey(workObject.Encryption.Decrypt(headerSplit[1], workObject.Encryption.GetServerPrivateKey())); Send(workObject, true.ToString()); }

            return returnValue;
        }

        #region GetPublicServerKey



        #endregion GetPublicServerKey

        #endregion InternalServerCommands

        #region Dispose

        public void Dispose()
        {
            if (Serverlistener != null)
            { continueListening = false; }
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
