using System.Collections.Generic;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class DeleteServer : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("deleteserver");
            activeStrings.Add("del");
            activeStrings.Add("delete");

            return activeStrings;
        }

        public (string, string) GetHelpCommandLog()
            => ("Deleteserver [ServerName]", "Deltes the selected server from UniversalServer");

        public string GetName()
            => "DeleteServer";

        public void RunCommand(string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}
