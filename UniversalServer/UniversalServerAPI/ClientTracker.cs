using Moonbyte.Logging;
using MoonbyteSettingsManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalServerAPI
{
    public class ClientTracker
    {
        #region Vars

        public string userID;
        public string userEndpoint;
        public MSMVault ClientStorage = new MSMVault();

        private bool isLoggedIn = false;
        private string[] Seperators = new string[] { "%80%" };
        private string internalSeperator = "%60%";

        #region SettingTitles

        private const string _UserLoginDates = "UserLoginDates";
        private const string _UserDisconnectedDates = "UserDisconnectedDates";

        #endregion SettingTitles

        #region Directories

        public string UserDirectory;

        private string UserDirectories;

        #endregion Directories

        #endregion Vars

        #region Properties

        public bool IsLoggedIn
        { get { return this.isLoggedIn; }
            set { isLoggedIn = value; } }

        #endregion Properties

        #region Initialize

        public ClientTracker(string ServerName)
        {
            UserDirectories = Environment.CurrentDirectory + @"\Servers\" + ServerName + @"\Users";

            if (!Directory.Exists(UserDirectories)) Directory.CreateDirectory(UserDirectories);
        }

        #endregion Initialize

        #region SetID

        public void SetID(string UserID, string UserEndpoint)
        {
            userID = UserID;
            userEndpoint = UserEndpoint;

            UserDirectory = UserDirectories + @"\" + userID;

            if (!Directory.Exists(UserDirectory)) Directory.CreateDirectory(UserDirectory);

            ClientStorage.ShowLog = false;
            ClientStorage.SettingsFileName = "UserData.dat";
            ClientStorage.SettingsDirectory = UserDirectory;

            this.IsLoggedIn = true;

            if (ClientStorage.CheckSetting(_UserLoginDates))
            {
                List<string> oldData = ClientStorage.ReadSetting(_UserLoginDates).Split(Seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
                oldData.Add(DateTime.Now.ToString() + internalSeperator + userEndpoint);

                string newData = String.Join(Seperators[0], oldData);
                ClientStorage.EditSetting(_UserLoginDates, newData);
            }
            else
            {
                string newData = DateTime.Now.ToString() + internalSeperator + userEndpoint;
                ClientStorage.EditSetting(_UserLoginDates, newData);
            }

            ILogger.AddToLog("INFO", "Client [" + userEndpoint + "] has logged in with ID [" + userID + "]");
        }

        #endregion SetID

        #region LogDisconnect

        public void LogDisconnect()
        {
            if (ClientStorage.CheckSetting(_UserDisconnectedDates))
            {
                List<string> oldData = ClientStorage.ReadSetting(_UserDisconnectedDates).Split(Seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
                oldData.Add(DateTime.Now.ToString() + internalSeperator + userEndpoint);

                string newData = String.Join(Seperators[0], oldData);
                ClientStorage.EditSetting(_UserDisconnectedDates, newData);
            }
            else
            {
                string newData = DateTime.Now.ToString() + internalSeperator + userEndpoint;
                ClientStorage.EditSetting(_UserDisconnectedDates, newData);
            }

            ILogger.AddToLog("INFO", "Client [" + userEndpoint + "] has disconnected with id [" + userID + "]");
        }

        #endregion LogDisconnect
    }
}
