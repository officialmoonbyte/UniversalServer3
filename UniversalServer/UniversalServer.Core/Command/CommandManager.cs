using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using Moonbyte.UniversalServer.Core.Server;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace UniversalServer.Core.Command
{

    /// <summary>
    /// => UniversalGetPacket
    /// 
    /// This region is all about processing the UniversalGetPacket data
    /// and redirecting it to the servers plugins.
    /// 
    /// => UniversalPacket
    /// 
    /// Unlike UniversalGetPacket, UniversalPackets are encrypted, can be get and post
    /// commands, and can have extremely complex plugins. Therefore we have another
    /// method for handling it since its harder to process.
    /// </summary>
    public class CommandManager : IDisposable
    {

        #region UniversalGetPacket

        public void ProcessUniversalGetPacketCommand(string messageData, ClientWorkObject client, AsynchronousSocketListener server)
        {
            server.Send(client, "Unsupported Request");
        }

        #endregion UniversalGetPacket

        #region UniversalPacket

        public bool ProcessUniversalPacketCommand(UniversalPacket universalPacket, ClientWorkObject client, AsynchronousSocketListener server)
        {
             bool sentClientData = false;

            string[] commandArgs = ((string[])JsonConvert.DeserializeObject<string[]>(universalPacket.MessageData.Data));

            foreach (IUniversalCommand universalCommand in UniversalCommand.GetDefaultCommands())
            {
                if (universalCommand.GetNamespace().ToLower() == commandArgs[0].ToLower())
                {
                    bool tmpbool = universalCommand.Invoke(commandArgs, universalPacket, client);
                    server.serverPluginLogger.AddToLog("INFO", "Client [" + client.clientTracker.userID + "] has used command [" + universalCommand.GetNamespace() + ".");
                    if (tmpbool == true) { sentClientData = true; return tmpbool; }
                }
            }

            if (sentClientData) return sentClientData;

            //Process plugins

            return sentClientData;
        }

        #endregion UniversalPacket
        public void Dispose() { }
    }
}
