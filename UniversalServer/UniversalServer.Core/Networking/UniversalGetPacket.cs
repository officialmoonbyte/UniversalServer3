using Newtonsoft.Json;

namespace UniversalServer.Core.Networking
{
    public class UniversalGetPacket : IUniversalPacket
    {

        #region Vars
        public enum HTTPSTATUS { GET, SET }
        public Get_Header MessageHeader = new Get_Header();
        public Get_Message MessageData = new Get_Message();

        #endregion Vars

        #region Initialization
        public UniversalGetPacket(Get_Header Messageheader, Get_Message Messagedata)
        {
            MessageHeader = Messageheader;
            MessageData = Messagedata;
        }

        #endregion Initialization

        #region ToString
        public override string ToString() => JsonConvert.SerializeObject(MessageHeader) + "." + JsonConvert.SerializeObject(MessageData.Data);

        #endregion ToString
    }

    public class Get_Header
    {
        public string type;
        public readonly UniversalPacket.HTTPSTATUS status = UniversalPacket.HTTPSTATUS.GET;
    }

    public class Get_Message
    {
        public object Data;
    }
}
