using System.Collections.Generic;
using UniversalServer.Commandline.ConsoleCommands;
using UniversalServer.Interfaces;

namespace UniversalServer.Invokers
{
    public class DefaultCommandInvoker : ICommandInvoker
    {
        public List<IConsoleCommand> ConstructConsoleCommands()
        {
            var commands = new List<IConsoleCommand>();

            commands.Add(new CreateServer());
            commands.Add(new InvokeConsoleCommand());
            commands.Add(new ListServer());
            commands.Add(new StartServer());
            commands.Add(new StopServer());

            return commands;
        }
    }
}
