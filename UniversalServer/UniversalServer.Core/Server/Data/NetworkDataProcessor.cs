using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using Newtonsoft.Json;
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

        }

        private IUniversalPacket getUniversalPacket(string[] dataReceived)
        {
            Header header = JsonConvert.DeserializeObject<Header>(dataReceived[0]);
            Message message = JsonConvert.DeserializeObject<Message>(dataReceived[1]);
            Signature signature = JsonConvert.DeserializeObject<Signature>(dataReceived[2]);

            return new UniversalPacket(header, message, signature);
        }

        #endregion Private Methods
    }
}
