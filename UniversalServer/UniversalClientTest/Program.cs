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
            client.ConnectToRemoteServer("192.168.0.16", 7876);

            client.SendCommand("userdatabase", new string[] { "editvalue", "moonbyte", "tempProfile", "VermeerVersion", "1.0.0"});

            Console.Read();
        }
    }
}
