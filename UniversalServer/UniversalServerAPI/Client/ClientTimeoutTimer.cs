using System;
using System.Threading.Tasks;

namespace Moonbyte.UniversalServerAPI.Client
{
    public class ClientTimeout
    {

        #region Vars

        public bool IsDisposed = false;
        ClientWorkObject clientSocket;

        #endregion Vars

        #region Initialization

        public ClientTimeout(ClientWorkObject WorkObject)
        {
            clientSocket = WorkObject; ClientCheckLoop();
        }

        #endregion Initialization

        #region Asynchronous Loop

        private void ClientCheckLoop()
        {
            Task.Run(async () =>
            {
                while (clientSocket.clientSocket.Connected)
                {
                    if (clientSocket.clientSocket.Connected && this.IsDisposed == false)
                    {

                    }
                    else
                    {
                        clientSocket.Dispose();
                        break;
                    }
                    await Task.Delay(600000);
                }
            });

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion Asynchronous Loop

        #region Dispose

        public void Dispose()
        { this.IsDisposed = true; }

        #endregion Dispose
    }
}
