using System.Collections.Generic;

namespace UniversalServer.Interfaces
{
    public interface IConsoleCommand
    {
        List<string> GetActiveStrings();

        void RunCommand(string[] args);
    }
}
