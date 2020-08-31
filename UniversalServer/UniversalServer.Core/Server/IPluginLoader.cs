/* Huge thanks to this post : https://www.codeproject.com/Articles/1052356/Creating-a-Simple-Plugin-System-with-NET
A lot of source code was used from that post that wasn't modified, so I thought it was a good idea to leave this
post credited, instead of just taking the work. */

using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Moonbyte.UniversalServer.Core.Server
{
    public class IPluginLoader
    {

        #region Vars

        List<UniversalPlugin> returnPlugins = new List<UniversalPlugin>();

        #endregion Vars

        #region LoadPlugins

        public List<UniversalPlugin> LoadPlugins(string PluginDirectory)
        {
            returnPlugins = new List<UniversalPlugin>();
            string pluginDataDirectory = Path.Combine(PluginDirectory, "Data");

            ILogger.AddWhitespace();

            if (!Directory.Exists(pluginDataDirectory)) Directory.CreateDirectory(pluginDataDirectory);

            if (Directory.Exists(PluginDirectory))
            {
                string[] files = Directory.GetFiles(PluginDirectory);
                foreach(string file in files)
                {
                    if (file.EndsWith(".dll"))
                    { Assembly.LoadFile(Path.GetFullPath(file)); }
                }
            } else { Directory.CreateDirectory(PluginDirectory); }

            Type interfaceType = typeof(IUniversalPlugin);
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass).ToArray();
            foreach (Type type in types)
            {
                try
                {
                    IUniversalPlugin plugin = (IUniversalPlugin)Activator.CreateInstance(type);

                    UniversalPlugin pluginInstance = new UniversalPlugin(plugin);
                    pluginInstance.core = plugin;

                    ILogger.AddToLog("INFO", "Initializing [" + pluginInstance.core.Name + "]");
                    pluginInstance.core.SetUniversalPluginAPI(pluginInstance);
                    pluginInstance.core.Initialize(Path.Combine(pluginDataDirectory, pluginInstance.core.Name), pluginInstance);

                    returnPlugins.Add(pluginInstance);
                    ILogger.AddToLog("INFO", "Plugin [" + pluginInstance.core.Name + "] Fully loaded!");
                }
                catch (Exception e)
                {
                    //Tried to load the assembly instance
                }
            }

            ILogger.AddWhitespace();

            return returnPlugins;
        }

        #endregion LoadPlugins

        #region Dispose

        public void Dispose()
        {
            returnPlugins = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion Dispose

    }
}
