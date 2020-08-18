using Moonbyte.UniversalServerAPI.Plugin.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonbyte.UniversalServerAPI.Plugin
{
    public class UniversalPlugin
    {
        #region Vars

        public IUniversalPlugin core;

        #endregion Vars

        #region Modules

        public ILogger Log = new ILogger();
        public EventManager eventManager = new EventManager();

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
