using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Server;
using System;

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
            bool clientSentData = false;
            foreach(IUniversalGetCommand getCommand in UniversalGetCommand.GetDefaultGetCommands())
            {
                if (getCommand.Name().ToLower() == messageData) { server.Send(client, getCommand.Get(), false); clientSentData = true; }
            }

            if (clientSentData == false)
            {
                server.Send(client, "Unknowncommand " + messageData, false);
                server.serverPluginLogger.AddToLog("UGETPacket", "Client tried to use [" + messageData + "], returned with Unknown command error");
            }
        }

        #endregion UniversalGetPacket

        #region UniversalPacket



        #endregion UniversalPacket
        public void Dispose() { }
    }
}
