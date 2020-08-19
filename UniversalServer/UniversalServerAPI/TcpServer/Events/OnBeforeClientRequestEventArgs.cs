namespace Moonbyte.UniversalServerAPI.TcpServer.Events
{
    public class OnBeforeClientRequestEventArgs
    {
        public ServerAPI.moonbyteCancelRequest CancelRequest = ServerAPI.moonbyteCancelRequest.Continue;
        public string ErrorMessage = "USER_PLUGINAUTH";
        public string RawData;
        public ClientWorkObject Client;
    }
}
