using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalClient;

namespace UniversalClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Universalclient client = new Universalclient(true);
            client.ConnectToRemoteServer("127.0.0.1", 7876);

            client.SendCommand("userdatabase", new string[] { "getvalue", "testvalue"});

            Console.Read();
        }
    }
}
