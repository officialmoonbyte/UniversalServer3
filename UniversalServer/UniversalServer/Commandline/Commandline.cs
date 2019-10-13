using System.Threading;

namespace UniversalServer.Commandline
{
    public class Commandline
    {

        #region Vars

        private bool IsDisposed = false;

        #endregion Vars

        #region Initialization

        public Commandline()
        {

        }

        #endregion Initialization

        #region Commandline Thread

        private void InitializeCommandlineThread()
        {
            new Thread(new ThreadStart(() =>
            {
                while (!IsDisposed)
                {
                    
                }
            })).Start();
        }

        #endregion Commandline Thread

        #region Dispose

        public void Dispose()
        { IsDisposed = true; }

        #endregion Dispose
    }
}
