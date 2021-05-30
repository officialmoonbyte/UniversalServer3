using System.Collections.Generic;
using UniversalServer.Commandline.ConsoleCommands;
using UniversalServer.Interfaces;

namespace UniversalServer.Invokers
{
    public class DefaultCommandInvoker : ICommandInvoker
    {
        public (List<IConsoleCommand>, List<string>) ConstructConsoleCommands()
        {
            var commands = new List<IConsoleCommand>();

            commands.Add(new CreateServer());
            commands.Add(new InvokeConsoleCommand());
            commands.Add(new ListServer());
            commands.Add(new StartServer());
            commands.Add(new StopServer());
            commands.Add(new Help());
            commands.Add(new UPD());
            commands.Add(new DeleteServer());

            return (commands, new List<string>());
        }
    }
}
