using Moonbyte.UniversalServer.Core.Plugin.module;
using Moonbyte.UniversalServer.Plugin.Module;

namespace Moonbyte.UniversalServer.Core.Plugin
{
    public class UniversalPlugin
    {
        #region Vars

        public IUniversalPlugin core;

        #endregion Vars

        #region Modules

        public Logger Log = new Logger();
        public PluginEventManager EventManager = new PluginEventManager();

        #endregion Modules

        #region Initialization

        public UniversalPlugin(IUniversalPlugin InterfaceClass)
        {
            core = InterfaceClass;
            core.SetUniversalPluginAPI(this);
        }

        #endregion Initialization

    }
}
