using System;
using System.Threading.Tasks;
using Moonbyte.Networking;
using Moonbyte.UniversalServer.Core.Networking;
using Newtonsoft.Json;
using UniversalServer.Core.Networking;

namespace UniversalClientTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            UniversalClient client = new UniversalClient();
            client.ConnectToRemoteServer("127.0.0.1", 7876);

            UniversalPacket updateCheck = new UniversalPacket(
                new Header() { status = UniversalPacket.HTTPSTATUS.GET },
                new Message() { Data = JsonConvert.SerializeObject(new string[] { "userdatabase", "getvalue", "VermeerCurrentVersion" }), IsEncrypted = false },
                client.GetSignature);
            UniversalServerPacket LatestVersion = client.SendMessage(updateCheck);

            Console.Read();
        }
    }
}
