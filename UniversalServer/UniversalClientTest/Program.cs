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
        static string serverIP = "127.0.0.1";
        static int serverPort = 7876;

        static async Task Main(string[] args)
        {
            UniversalClient _client = GetUniversalClient();

            string valueTitle = "VermeerVersion";

            UniversalPacket getValueCommand = new UniversalPacket(
                new Header() { status = UniversalPacket.HTTPSTATUS.GET },
                new Message() { Data = JsonConvert.SerializeObject(new string[] { "userdatabase", "getvalue", valueTitle }), IsEncrypted = true },
                _client.GetSignature);
            UniversalServerPacket serverReturn = _client.SendMessage(getValueCommand);

            switch (serverReturn.Message.ToUpper())
            {
                case "UNAUTHORIZED":
                    break;
                case "UNKNOWNCOMMAND":
                    break;
                case "USRDBS_GETVALUE_VALUEEXIST":
                    break;
                default:
                    break;
            }
        }

        private static UniversalClient GetUniversalClient()
        {
            UniversalClient client = new UniversalClient();
            client.ConnectToRemoteServer(serverIP, serverPort);

            return client;
        }
    }
}
