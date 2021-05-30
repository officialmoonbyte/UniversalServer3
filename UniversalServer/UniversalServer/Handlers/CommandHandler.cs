using System.Collections.Generic;
using System.Linq;
using UniversalServer.Interfaces;
using UniversalServer.Invokers;

namespace UniversalServer.Handlers
{
    public class CommandHandler
    {

        #region Vars

        private List<IConsoleCommand> consoleCommands = new List<IConsoleCommand>();
        private List<string> registeredConsoleCommands = new List<string>();

        #endregion Vars

        #region Constructor

        public CommandHandler()
        {
            // Default Commands
            var defaultCommandInvoker = new DefaultCommandInvoker();

            var constructedDefaultCommands = defaultCommandInvoker.ConstructConsoleCommands();

            consoleCommands = constructedDefaultCommands.Item1;
            registeredConsoleCommands = constructedDefaultCommands.Item2;
        }

        #endregion Constructor

        #region HandleCommand

        public void HandleCommand(string[] args)
        {
            var command = consoleCommands.FirstOrDefault(x => x.GetActiveStrings().Contains(args[0].ToLower()));

            if (command == null)
                return;

            command.RunCommand(args);
        }

        #endregion HandleCommand

        #region GetCommands

        public List<IConsoleCommand> GetCommands() => consoleCommands;

        #endregion GetCommands
    }
}
