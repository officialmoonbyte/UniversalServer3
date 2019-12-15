using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalServerAPI
{
    public class ClientTracker
    {
        #region Vars

        private bool isLoggedIn = false;

        #endregion Vars

        #region Properties

        public bool IsLoggedIn
        { get { return this.isLoggedIn; }
            set { isLoggedIn = value; } }

        #endregion Properties
    }
}
