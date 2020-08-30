using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Networking;
using System.Collections.Generic;
using UniversalServer.Core.Command.Commands.ServerRequest.Encryption;

namespace UniversalServer.Core.Command
{
    public interface IUniversalCommand
    {
        public string GetNamespace();
        public bool Invoke(string[] commandArgs, UniversalPacket universalPacket, ClientWorkObject client);
    }

    public static class UniversalCommand
    {
        public static List<IUniversalCommand> GetDefaultCommands()
        {
            List<IUniversalCommand> returnList = new List<IUniversalCommand>();

            returnList.Add(new GetServerPublicKey());
            returnList.Add(new SetClientPrivateKey());
            returnList.Add(new SetClientPublicKey());

            return returnList;
        }
    }
}
