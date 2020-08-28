using Moonbyte.UniversalServer.Core.Client;
using static Moonbyte.UniversalServer.Core.Model.Utility;

namespace Moonbyte.UniversalServer.Core.Server.Events
{
    public class OnBeforeClientRequestEventArgs
    {
        public MoonbyteCancelRequest CancelRequest = MoonbyteCancelRequest.Continue;
        public string ErrorMessage = "USER_PLUGINAUTH";
        public string RawData;
        public ClientWorkObject Client;
    }
}
