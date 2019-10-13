using Moonbyte.UniversalServer.TcpServer;
using System.Net.Sockets;
using System.Text;

namespace Moonbyte.UniversalServerAPI
{
    public class ClientWorkObject
    {

        #region Vars

        #region Network Objects

        public Socket clientSocket = null;
        public AsynchronousSocketListener serverSocket = null;
        public ClientSender clientSender;

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
            clientTimeoutTimer = new ClientTimeout(this);
        }

        #endregion Initialization

        #region Buffer

        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];

        #endregion Buffer 

        #region Other

        public StringBuilder sb = new StringBuilder();

        #endregion Other

        #region Dispose

        public void Dispose()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            clientTimeoutTimer.Dispose();
        }

        #endregion Dispose
    }
}
