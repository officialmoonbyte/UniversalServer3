using System.Collections.Generic;

namespace UniversalServer.Interfaces
{
    public interface IConsoleCommand
    {
        string GetName();
        (string, string) GetHelpCommandLog(); 
        List<string> GetActiveStrings();
        void RunCommand(string[] args);
    }
}
