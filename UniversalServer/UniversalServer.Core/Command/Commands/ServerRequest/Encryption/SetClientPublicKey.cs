using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using UniversalServer.Core.Globalization;

namespace UniversalServer.Core.Command.Commands.ServerRequest.Encryption
{
    public class SetClientPublicKey : IUniversalCommand
    {
        public string GetNamespace() => "ServerRequest.Encryption.SetClientPublicKey";
        public bool Invoke(string[] CommandArgs, UniversalPacket universalPacket, ClientWorkObject client)
        {
            if (universalPacket.MessageHeader.status == UniversalPacket.HTTPSTATUS.POST)
            {
                client.Encryption.SetClientPublicKey(CommandArgs[1]);
                client.serverSocket.Send(client, true.ToString());
            }
            else 
            {
                client.serverSocket.Send(client, Messages.PostStatusCodeRequiredErrorMessage);
            }
            return true;
        }
    }
}
