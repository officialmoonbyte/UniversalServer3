using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using Moonbyte.UniversalServer.Core.Server;
using System;
using UniversalServer.Core.Command;

namespace UniversalServer.Core.Server.Data
{
    public class PostProcessingUniversalGetPacket : IDisposable
    {

        #region PostProcessPacket

        public void PostProcessPacket(UniversalGetPacket packet, ClientWorkObject client, AsynchronousSocketListener server) 
        { 
            if (Type.GetType(packet.MessageHeader.type) == typeof(string))
            {
                string messageData = (string)packet.MessageData.Data;
                using (CommandManager commandManager = new CommandManager(server))
                {
                    commandManager.ProcessUniversalGetPacketCommand(messageData, client, server);
                }
            }
        }

        #endregion PostProcessPacket

        #region Dispose

        public void Dispose()
        {
            
        }

        #endregion Dispose

    }
}
