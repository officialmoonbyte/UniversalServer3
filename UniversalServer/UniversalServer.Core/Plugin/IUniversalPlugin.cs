using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Plugin.Module;

namespace Moonbyte.UniversalServer.Core.Plugin
{
    /// <summary>
    /// Interface for developers to develop plugins for UniversalServer
    /// </summary>
    public interface IUniversalPlugin
    {
        string Name { get; }

        UniversalPlugin GetUniversalPluginAPI();
        void SetUniversalPluginAPI(UniversalPlugin Plugin);
        bool Initialize(string PluginDataDirectory, UniversalPlugin BaseClass);
        bool Invoke(ClientWorkObject clientObject, string[] commandArgs);
        void ConsoleInvoke(string[] commandArgs, Logger iLogger);

    }
}
