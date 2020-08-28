using System;

namespace UniversalServer.Core.Command.UniversalGetPacketCommands
{
    public class datetime : IUniversalGetCommand
    {

        public string Name() => "datetime";

        public void Initialize()
        {
            
        }

        public string Get() => new DateTime().ToString();
    }
}
