using Moonbyte.UniversalServer.Core.Server.Events;
using System;

namespace Moonbyte.UniversalServer.Core.Plugin.module
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
