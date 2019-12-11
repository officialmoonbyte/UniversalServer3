using System;
using System.Threading;

namespace UniversalServer.Commandline
{
    public class CommandLine
    {

        #region Vars

        private bool IsDisposed = false;

        #endregion Vars

        #region Initialization

        public CommandLine()
        { InitializeCommandlineThread(); }

        #endregion Initialization

        #region Commandline Thread

        private void InitializeCommandlineThread()
        {
            new Thread(new ThreadStart(() =>
            {
                while (!IsDisposed)
                {
                    //Gets the user console input - Not for socket network transfer! Only local commands.
                    string _Userinput = Console.ReadLine();
                    string[] UserInput = _Userinput.Split(null);

                    //Processes the command through the UniversalServerCommandLine object, this object is initialized in the static class Universalserver.
                    Universalserver.ProcessCommand(UserInput); // UserInput is a diffrent object from _Userinput. _Userinput is just the raw string and ProcessCommand needs a array.
                }
            })).Start();
        }

        #endregion Commandline Thread

        #region Dispose

        /// <summary>
        /// Set the IsDispose var to true and stops the while loop and closes the application.
        /// </summary>
        public void Dispose()
        { IsDisposed = true; Universalserver.Close(); }

        #endregion Dispose
    }
}
