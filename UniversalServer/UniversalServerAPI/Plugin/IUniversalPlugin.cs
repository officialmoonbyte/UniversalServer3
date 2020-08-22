using Moonbyte.UniversalServerAPI.Client;

namespace Moonbyte.UniversalServerAPI.Plugin
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
