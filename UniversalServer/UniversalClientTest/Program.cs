using System;
using Moonbyte.Networking;

namespace UniversalClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            UniversalClient client = new UniversalClient();
            client.ConnectToRemoteServer("localhost", 7876);

            Console.Read();
        }
    }
}
