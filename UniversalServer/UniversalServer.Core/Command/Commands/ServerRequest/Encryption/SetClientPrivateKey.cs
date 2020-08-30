using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using UniversalServer.Core.Globalization;

namespace UniversalServer.Core.Command.Commands.ServerRequest.Encryption
{
    public class SetClientPrivateKey : IUniversalCommand
    {
        public string GetNamespace() => "ServerRequest.Encryption.SetClientPrivateKey";

        public bool Invoke(string[] CommandArgs, UniversalPacket universalPacket, ClientWorkObject client)
        {
            if (universalPacket.MessageHeader.status == UniversalPacket.HTTPSTATUS.POST)
            {
                client.Encryption.SetClientPrivateKey(CommandArgs[1]);
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
