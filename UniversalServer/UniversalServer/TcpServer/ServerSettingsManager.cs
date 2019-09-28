using GlobalSettingsFramework;
using System;
using System.IO;

namespace Moonbyte.UniversalServer.TcpServer
{
    public class ServerSettingsManager
    {

        #region Setting Vars

        #region Setting Names

        private const string _ServerPortName = "ServerPort";

        #endregion Setting Names

        #region Setting Values

        public int ServerPort;

        #endregion Setting Values

        #region Default Values

        private const int _ServerPortValue = 7777;

        #endregion Default Values

        #endregion Setting Vars

        #region Vars

        GFS SettingsFramework;

        #endregion Vars

        #region Initialization

        public ServerSettingsManager(string SettingsDirectory)
        {
            if (Directory.Exists(SettingsDirectory)) Directory.CreateDirectory(SettingsDirectory);
            SettingsFramework = new GFS(SettingsDirectory + @"\ServerSettings.ini");
        }

        #endregion Initialization

        #region Initialize Setting Values

        private void InitializeSettingValues()
        {
            if (SettingsFramework.CheckSetting(_ServerPortName))
            { ServerPort = int.Parse(SettingsFramework.ReadSetting(_ServerPortName)); }
            else { SettingsFramework.EditSetting(_ServerPortName, _ServerPortValue.ToString()); ServerPort = _ServerPortValue; }
        }

        #endregion Initialize Setting Values

        #region Dispose

        public void Dispose()
        {
            this.SaveSettings();
            SettingsFramework = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion Dispose

        #region SaveSettings

        public void SaveSettings()
        {
            SettingsFramework.EditSetting(_ServerPortName, ServerPort.ToString());
        }

        #endregion SaveSettings
    }
}
