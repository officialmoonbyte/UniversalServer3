namespace Moonbyte.UniversalServerAPI.Client
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
        public bool Send(ClientWorkObject WorkObject, string Data)
        { WorkObject.serverSocket.Send(WorkObject, Data); return true; }

        #endregion Send
    }
}
