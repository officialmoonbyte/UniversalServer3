using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Plugin.Module;
using System.Collections.Generic;
using UniversalServer.Interfaces;
using static Moonbyte.UniversalServer.Core.Logging.ILogger;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class Help : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("help");
            activeStrings.Add("h");

            return activeStrings;
        }

        public (string, string) GetHelpCommandLog()
            => ("Help", "Displays all of the commands installed on the server!");

        public string GetName()
            => "Help";

        public void RunCommand(string[] args)
        {
            var commands = Universalserver.CommandHandler.GetCommands();

            ILogger.AddWhitespace();
            commands.ForEach(x =>
            {
                var commandLog = x.GetHelpCommandLog();

                ILogger.AddToLog(Levels.INFO, $"{commandLog.Item1} - {commandLog.Item2}");
            });
            ILogger.AddWhitespace();
        }
    }
}
