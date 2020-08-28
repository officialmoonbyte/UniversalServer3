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
            if (universalPacket.GetType() == typeof(UniversalGetPacket))
            {
                using (PostProcessingUniversalGetPacket postProcessing = new PostProcessingUniversalGetPacket())
                {
                    UniversalGetPacket packet = (UniversalGetPacket)universalPacket;
                    postProcessing.PostProcessPacket(packet, workObject, parent);
                }
            }
            else if (universalPacket.GetType() == typeof(UniversalPacket))
            {
                UniversalPacket packet = (UniversalPacket)universalPacket;
            }
            else
            {
                //returns error
            }
        }

        private IUniversalPacket getUniversalPacket(string[] dataReceived)
        {
            Get_Header header = JsonConvert.DeserializeObject<Get_Header>(dataReceived[0]);
            Get_Message message = JsonConvert.DeserializeObject<Get_Message>(dataReceived[1]);

            return new UniversalGetPacket(header, message);
        }

        #endregion Private Methods
    }
}
