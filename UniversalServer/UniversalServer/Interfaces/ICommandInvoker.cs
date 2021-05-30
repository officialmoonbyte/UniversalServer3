using System.Collections.Generic;

namespace UniversalServer.Interfaces
{
    public interface ICommandInvoker
    {
        (List<IConsoleCommand>, List<string>) ConstructConsoleCommands();
    }
}
