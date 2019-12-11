using Moonbyte.Logging;
using Moonbyte.UniversalServerAPI;
using MoonbyteSettingsManager;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        Socket listener;

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

            // Get the IPAddress / endpoint for the socket
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ServerPort);

            // Initializes a Tcp/IP socket.
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            new Thread(new ThreadStart(() =>
            {
                // Bind the socket and then start listening for incoming connections.
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(550);

                    continueListening = true;

                    while (continueListening)
                    {
                        isListening = true;

                        allDone.Reset();

                        listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);

                        allDone.WaitOne();
                    }

                    //Disposes the listener
                    isListening = false;

                    listener.Close();
                    listener.Dispose();
                }
                catch (Exception e)
                { ILogger.LogExceptions(e); }
            })).Start();
        }

        #endregion StartListening

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
                    //Command Handler

                    //UserLog
                }
                else
                {
                    //Not all data received, get more
                    handler.BeginReceive(workObject.buffer, 0, ClientWorkObject.BufferSize, 0, new AsyncCallback(ReadCallback), workObject);
                }
            }
        }

        #endregion ReadCallback

        #region Send

        public void Send(ClientWorkObject WorkObject, string Data)
        {
            // Gets the socket from the work object.
            Socket handler = WorkObject.clientSocket;

            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(Data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        #endregion Send

        #region Dispose

        public void Dispose()
        {
            if (listener != null)
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
