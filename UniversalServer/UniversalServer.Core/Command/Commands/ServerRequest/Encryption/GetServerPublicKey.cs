using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;

namespace UniversalServer.Core.Command.Commands.ServerRequest.Encryption
{
    public class GetServerPublicKey : IUniversalCommand
    {

        public bool Invoke(string[] commandArgs, UniversalPacket universalPacket, ClientWorkObject client)
        {
            client.serverSocket.Send(client, client.Encryption.GetServerPublicKey(), false);
            return true;
        }

        public string GetNamespace() => "ServerRequest.Encryption.GetServerPublicKey";
    }
}
