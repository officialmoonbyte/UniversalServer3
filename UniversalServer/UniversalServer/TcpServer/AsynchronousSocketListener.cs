using Moonbyte.UniversalServerAPI;
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

        public string serverName;
        private ServerSettingsManager ServerSettings;
        public ManualResetEvent allDone = new ManualResetEvent(false);

        #endregion Vars

        #region Initialization

        public AsynchronousSocketListener(string ServerName)
        {
            serverName = ServerName;
            ServerSettings = new ServerSettingsManager(Environment.CurrentDirectory + @"\Servers\" + ServerName);
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
                string dir = StartupDirectory + @"\" + this.serverName;
                if (Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        #endregion GetServerDirectory

        #region InitializeServerSettings



        #endregion InitializeServerSettings

        #region StartListening

        public void StartListening()
        {
            //Gets the port
            int ServerPort = ServerSettings.ServerPort;

            // Get the IPAddress / endpoint for the socket
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ServerPort);

            // Initializes a Tcp/IP socket.
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket and then start listening for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(550);

                while (true)
                {
                    allDone.Reset();

                    listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString()); }
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
            ClientWorkObject state = new ClientWorkObject();
            state.clientSocket = handler;
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

        private void Send(ClientWorkObject WorkObject, string Data)
        {
            // Gets the socket from the work object.
            Socket handler = WorkObject.clientSocket;

            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(Data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        #endregion Send

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
