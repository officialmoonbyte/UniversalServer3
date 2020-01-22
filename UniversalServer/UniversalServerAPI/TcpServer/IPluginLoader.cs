/* Huge thanks to this post : https://www.codeproject.com/Articles/1052356/Creating-a-Simple-Plugin-System-with-NET
A lot of source code was used from that post that wasn't modified, so I thought it was a good idea to leave this
post credited, instead of just taking the work. */

using Moonbyte.Logging;
using Moonbyte.UniversalServerAPI.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Moonbyte.UniversalServer.TcpServer
{
    public class IPluginLoader
    {

        #region Vars

        List<IUniversalPlugin> returnPlugins = new List<IUniversalPlugin>();

        #endregion Vars

        #region LoadPlugins

        public List<IUniversalPlugin> LoadPlugins(string PluginDirectory)
        {
            returnPlugins = new List<IUniversalPlugin>();
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
                IUniversalPlugin plugin = (IUniversalPlugin)Activator.CreateInstance(type);

                ILogger.AddToLog("INFO", "Initializing [" + plugin.Name + "]");
                plugin.Initialize(Path.Combine(pluginDataDirectory, plugin.Name));

                returnPlugins.Add(plugin);
                ILogger.AddToLog("INFO", "Plugin [" + plugin.Name + "] Fully loaded!");
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
