using System.Collections.Generic;
using UniversalServer.Core.Command.UniversalGetPacketCommands;

namespace UniversalServer.Core.Command
{
    public interface IUniversalGetCommand
    {
        public string Name();
        public void Initialize();
        public string Get();
    }

    public static class UniversalGetCommand
    {
        public static List<IUniversalGetCommand> GetDefaultGetCommands()
        {
            List<IUniversalGetCommand> returnList = new List<IUniversalGetCommand>();

            returnList.Add(new datetime());

            return returnList;
        }
    }
}
