using System;
using System.Collections.Generic;
using System.IO;

namespace UniversalServer
{
    public class universalServerSettingsManager
    {

        #region Vars



        #endregion Vars

        #region Default Vars

        public string UniversalServerSettingsDirectory;
        public const int DefaultServerPort = 7876;

        #endregion Default Vars

        #region Initialization

        public universalServerSettingsManager()
        {
            UniversalServerSettingsDirectory = Environment.CurrentDirectory + @"\ServerSettings.ini";

            if (!File.Exists(UniversalServerSettingsDirectory)) { File.Create(UniversalServerSettingsDirectory).Close(); }
        }

        #endregion Initialization

        #region SettingTitle's



        #endregion SettingTitle's

        #region Load

        /// <summary>
        /// Call's the internal UniversalServerSettingsManager.Load method
        /// Loads all of the settings based on the local file.
        /// </summary>
        public void ReloadSettings()
        { this.Load(); }

        private void Load()
        {
            string[] FileSettings = File.ReadAllLines(UniversalServerSettingsDirectory);
        }

        #endregion Load

        #region Save

        public void SaveCurrentSettings()
        {

        }

        #endregion Save

    }
}
