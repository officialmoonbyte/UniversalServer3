using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using Moonbyte.UniversalServer.Core.Server.Events;
using Newtonsoft.Json;
using System;
using System.Net;
using UniversalServer.Core.Command;
using UniversalServer.Core.Server.Data;

namespace Moonbyte.UniversalServer.Core.Server.Data
{
    public class NetworkDataProcessor
    {

        #region Vars

        AsynchronousSocketListener parent;

        #endregion Vars

        #region Initialization

        public NetworkDataProcessor(AsynchronousSocketListener parentServer)
        {
            parent = parentServer;
        }

        #endregion Initialization

        #region Public Methods

        public void ProcessDataReceived(ClientWorkObject workObject, string[] dataReceived) => postProcessUniversalPacket(workObject, getUniversalPacket(dataReceived));

        #endregion Public Methods

        #region Private Methods

        private void postProcessUniversalPacket(ClientWorkObject workObject, IUniversalPacket universalPacket)
        {
            UniversalPacket clientPacket = (UniversalPacket)universalPacket;

            //Checks if the user ID is registered (IsLoggedIn)
            //If the ID is not stored then stores the ID and
            //the IP of the client for logging reasons.
            if (!workObject.clientTracker.IsLoggedIn) workObject.clientTracker.SetID(clientPacket.MessageSignature.clientId, 
                                                        ((IPEndPoint)workObject.clientSocket.RemoteEndPoint).Address.ToString(), 
                                                        clientPacket.MessageSignature.clientIp);
            clientPacket.MessageHeader.dateTime = DateTime.Now; //Sets the datetime if its null

            //Decrypts the information
            if (clientPacket.MessageData.IsEncrypted) clientPacket.MessageData.Data = workObject.Encryption.Decrypt(clientPacket.MessageData.Data, workObject.Encryption.GetServerPrivateKey());

            OnBeforeClientRequestEventArgs onBeforeRequest = new OnBeforeClientRequestEventArgs { RawData = clientPacket.MessageData.Data, Client = workObject };
            parent.GetEventTracker().OnBeforeClientRequest(this, onBeforeRequest);

            //Processes the command
            bool sentClientData = false;

            if (onBeforeRequest.CancelRequest == Model.Utility.MoonbyteCancelRequest.Continue)
            {
                using (CommandManager commandManager = new CommandManager(parent))
                {
                    sentClientData = commandManager.ProcessUniversalPacketCommand(clientPacket, workObject, workObject.serverSocket);
                }
            }
            else
            {
                workObject.serverSocket.Send(workObject, "Unauthorized: A 3rd party software marked your request as unauthorized.", false);
            }

            //Sends data to client if false
            if (!sentClientData) workObject.serverSocket.Send(workObject, "Unknown Command!", false);
        }

        private IUniversalPacket getUniversalPacket(string[] dataReceived)
        {
            if (dataReceived == null) return null;
            Header header = JsonConvert.DeserializeObject<Header>(dataReceived[0]);
            Message message = JsonConvert.DeserializeObject<Message>(dataReceived[1]);
            message.IsEncrypted = bool.Parse(dataReceived[3]);
            Signature signature = JsonConvert.DeserializeObject<Signature>(dataReceived[2]);

            return new UniversalPacket(header, message, signature);
        }

        #endregion Private Methods
    }
}
