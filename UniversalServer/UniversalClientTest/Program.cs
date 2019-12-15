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
            client.ConnectToRemoteServer("192.168.0.2", 7876);
            client.SendCommand("testing", new string[] { "test", "123" });
            Console.Read();
        }
    }
}
