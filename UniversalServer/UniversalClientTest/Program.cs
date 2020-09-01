using System;
using System.Threading.Tasks;
using Moonbyte.Networking;

namespace UniversalClientTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            UniversalClient client = new UniversalClient();
            
            Console.Read();
        }
    }
}
