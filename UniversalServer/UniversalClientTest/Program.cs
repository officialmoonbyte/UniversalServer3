using System;
using Moonbyte.Networking;

namespace UniversalClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            UniversalClient client = new UniversalClient(true);
            client.ConnectToRemoteServer("192.168.0.16", 7876);

            client.SendCommand("userdatabase", new string[] { "editvalue", "moonbyte", "tempProfile", "VermeerVersion", "1.0.0"});

            Console.Read();
        }
    }
}
