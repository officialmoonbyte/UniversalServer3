using Moonbyte.UniversalServer.Core.Logging;

namespace UniversalServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initializes ILogger
            ILogger.SetLoggingEvents();

            //Initializes UniversalServer with the Universalserver class
            Universalserver.InitializeUniversalServer();
        }
    }
}
