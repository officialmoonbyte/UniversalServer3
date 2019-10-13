using System.Net.Sockets;
using System.Text;

namespace Moonbyte.UniversalServerAPI
{
    public class ClientSender
    {

        #region Vars

        ClientWorkObject ClientSocket;

        #endregion Vars

        #region Initialization

        public ClientSender(ClientWorkObject clientSocket)
        { ClientSocket = clientSocket; }

        #endregion Initialization

        #region Send
        public void Send(ClientWorkObject WorkObject, string Data)
        { WorkObject.serverSocket.Send(WorkObject, Data); }

        #endregion Send
    }
}
