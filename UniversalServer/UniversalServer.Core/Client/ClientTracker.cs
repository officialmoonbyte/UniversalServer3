using Moonbyte.UniversalServer.Core.Logging;
using MoonbyteSettingsManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Moonbyte.UniversalServer.Core.Client
{
    public class ClientTracker
    {
        #region Vars

        public string userID;
        public string userServerEndPoint;
        public string userClientEndPoint;
        public MSMVault ClientStorage;

        private bool isLoggedIn = false;
        private string[] Seperators = new string[] { "%80%" };
        private string internalSeperator = "%60%";

        #region SettingTitles

        private const string _UserLoginDates = "UserLoginDates";
        private const string _UserDisconnectedDates = "UserDisconnectedDates";

        #endregion SettingTitles

        #region Directories

        public string UserDirectory;
        public string UserKeyFile;

        private string userDirectories;

        #endregion Directories

        #endregion Vars

        #region Properties

        public bool IsLoggedIn
        { get { return this.isLoggedIn; }
            set { isLoggedIn = value; } }

        #endregion Properties

        #region Initialize

        public ClientTracker(string serverName)
        {
            userDirectories = Path.Combine(Environment.CurrentDirectory, "Servers", serverName, "Users");
            
            if (!Directory.Exists(userDirectories)) Directory.CreateDirectory(userDirectories);
        }

        #endregion Initialize

        #region SetID

        public void SetID(string UserID, string UserServerEndPoint, string UserClientEndPoint)
        {
            userID = UserID;
            userServerEndPoint = UserServerEndPoint;
            userClientEndPoint = UserClientEndPoint;

            UserDirectory = Path.Combine(userDirectories, userID);
            UserKeyFile = Path.Combine(UserDirectory, "key.key");

            if (!Directory.Exists(UserDirectory)) Directory.CreateDirectory(UserDirectory);

            ClientStorage = new MSMVault(UserKeyFile) 
            { 
                ShowLog = false,
                SettingsFileName = "UserData.dat",
                SettingsDirectory = UserDirectory
            };

            this.IsLoggedIn = true;

            if (ClientStorage.CheckSetting(_UserLoginDates))
            {
                List<string> oldData = ClientStorage.ReadSetting(_UserLoginDates).Split(Seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
                oldData.Add(DateTime.Now.ToString() + internalSeperator + userServerEndPoint);

                string newData = String.Join(Seperators[0], oldData);
                ClientStorage.EditSetting(_UserLoginDates, newData);
            }
            else
            {
                string newData = DateTime.Now.ToString() + internalSeperator + userServerEndPoint;
                ClientStorage.EditSetting(_UserLoginDates, newData);
            }

            ILogger.AddToLog("INFO", "Client [" + userServerEndPoint + "] has logged in with ID [" + userID + "]");
        }

        #endregion SetID

        #region LogDisconnect

        public void LogDisconnect()
        {
            if (ClientStorage.CheckSetting(_UserDisconnectedDates))
            {
                List<string> oldData = ClientStorage.ReadSetting(_UserDisconnectedDates).Split(Seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
                oldData.Add(DateTime.Now.ToString() + internalSeperator + userServerEndPoint);

                string newData = String.Join(Seperators[0], oldData);
                ClientStorage.EditSetting(_UserDisconnectedDates, newData);
            }
            else
            {
                string newData = DateTime.Now.ToString() + internalSeperator + userServerEndPoint;
                ClientStorage.EditSetting(_UserDisconnectedDates, newData);
            }

            ILogger.AddToLog("INFO", "Client [" + userServerEndPoint + "] has disconnected with id [" + userID + "]");
        }

        #endregion LogDisconnect
    }
}
