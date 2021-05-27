using System.Collections.Generic;

namespace UniversalServer.Interfaces
{
    public interface ICommandInvoker
    {
        List<IConsoleCommand> ConstructConsoleCommands();
    }
}
