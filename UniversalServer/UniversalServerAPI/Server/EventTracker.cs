using Moonbyte.UniversalServer.TcpServer;
using Moonbyte.UniversalServerAPI.Plugin;
using Moonbyte.UniversalServerAPI.TcpServer.Events;

namespace Moonbyte.UniversalServerAPI.TcpServer
{
    public class EventTracker
    {

        #region Vars

        AsynchronousSocketListener _parentSocketListener;

        #endregion Vars

        #region Initialization

        public EventTracker(AsynchronousSocketListener parentSocketListener)
        {
            _parentSocketListener = parentSocketListener;
        }

        #endregion Initialization

        #region Events

        #region OnBeforeClientRequest

        public void OnBeforeClientRequest(object sender, OnBeforeClientRequestEventArgs args)
        {
            foreach(UniversalPlugin plugin in _parentSocketListener.GetLoadedPlugins())
            {
                plugin.EventManager.InvokeOnBeforeClientRequest(sender, args);
            }
        }

        #endregion OnBeforeClientRequest

        #endregion Events

    }
}
