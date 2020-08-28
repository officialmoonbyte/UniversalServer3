using Moonbyte.UniversalServer.Core.Security;
using Moonbyte.UniversalServer.Core.Server;
using System.Net.Sockets;
using System.Text;

namespace Moonbyte.UniversalServer.Core.Client
{
    public class ClientWorkObject
    {

        #region Vars

        #region Network Objects

        public Socket clientSocket = null;
        public AsynchronousSocketListener serverSocket = null;
        public ServerRSA Encryption = new ServerRSA();
        public ClientSender clientSender;
        public ClientTracker clientTracker;

        #endregion Network Objects

        #region Tracking Objects / other services

        ClientTimeout clientTimeoutTimer;

        #endregion Tracking Objects

        #endregion Vars

        #region Initialization

        public ClientWorkObject(Socket ClientSocket, AsynchronousSocketListener ServerObject)
        {
            clientSocket = ClientSocket;
            serverSocket = ServerObject;

            clientSender = new ClientSender(this);
            clientTracker = new ClientTracker(serverSocket.ServerName);
            clientTimeoutTimer = new ClientTimeout(this);
        }

        #endregion Initialization

        #region Buffer

        public const int BufferSize = 1024;
        public int bufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];

        #endregion Buffer 

        #region Other

        public StringBuilder sb = new StringBuilder();

        #endregion Other

        #region Dispose

        public void Dispose()
        {
            clientTracker.LogDisconnect();
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            clientTimeoutTimer.Dispose();
        }

        #endregion Dispose
    }
}
