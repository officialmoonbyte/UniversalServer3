using Moonbyte.UniversalServerAPI.TcpServer.Events;
using System;

namespace Moonbyte.UniversalServerAPI.Plugin.module
{
    public class PluginEventManager
    {

        #region AsynchronousSocketListener

        #region Events

        public event EventHandler<OnBeforeClientRequestEventArgs> OnBeforeClientRequest;

        #endregion Events

        #region InvokeRequest

        public void InvokeOnBeforeClientRequest(object sender, OnBeforeClientRequestEventArgs args)
        {
            OnBeforeClientRequest?.Invoke(sender, args);
        }

        #endregion InvokeRequest

        #endregion AsynchronousSocketListener

    }
}
