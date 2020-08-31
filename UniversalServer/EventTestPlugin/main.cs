using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Plugin;
using Moonbyte.UniversalServer.Plugin.Module;
using System.IO;

namespace EventTestPlugin
{
    public class EventTestPlugin : IUniversalPlugin
    {

        #region Vars

        public string Name { get { return "EventTestPlugin"; } }


        #endregion Vars

        #region Basic area

        #region Invoke

        public bool Invoke(ClientWorkObject clientObject, string[] commandArgs)
        {
            if (commandArgs[1].ToUpper() == "TESTING")
            { return clientObject.clientSender.Send(clientObject, "Testing command activated"); }
            if (commandArgs[1].ToUpper() == "FOO")
            { return clientObject.clientSender.Send(clientObject, "Foo command activated"); }
            if (commandArgs[1].ToUpper() == "BAR")
            { return clientObject.clientSender.Send(clientObject, "Bar command activated"); }

            return false;
        }

        #endregion Invoke

        #region ConsoleInvoke

        public void ConsoleInvoke(string[] commandArgs, Logger iLogger)
        {
            
        }

        #endregion ConsoleInvoke

        #region Initialize Plugin

        public bool Initialize(string PluginDataDirectory, UniversalPlugin BaseClass)
        {
            if (!Directory.Exists(PluginDataDirectory)) Directory.CreateDirectory(PluginDataDirectory);

            BaseClass.EventManager.OnBeforeClientRequest += (obj, args) =>
            {
                ILogger.AddToLog("TESTINGTEST", args.RawData);
                args.CancelRequest = Moonbyte.UniversalServer.Core.Model.Utility.MoonbyteCancelRequest.Cancel;
            };

            return true;
        }

        #endregion Initialize Plugin

        #endregion Basic Area

        #region UniversalServerAPI

        UniversalPlugin ServerAPI;
        public UniversalPlugin GetUniversalPluginAPI()
        { return ServerAPI; }

        public void SetUniversalPluginAPI(UniversalPlugin Plugin)
        { ServerAPI = Plugin; }

        #endregion UniversalServerAPI

    }
}
