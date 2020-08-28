using Moonbyte.UniversalServer.TcpServer;
using Moonbyte.UniversalServerAPI.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UniversalServer.Core.Networking;
using Newtonsoft;
using Newtonsoft.Json;

namespace Moonbyte.UniversalServerAPI.Server.data
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
                UniversalGetPacket packet = (UniversalGetPacket)universalPacket;
                packet.ToString();
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
