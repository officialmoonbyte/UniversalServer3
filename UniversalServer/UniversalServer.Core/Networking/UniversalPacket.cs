using Newtonsoft.Json;
using System;

namespace UniversalServer.Core.Networking
{
    public class UniversalPacket : IUniversalPacket
    {

        #region Vars
        public enum HTTPSTATUS { GET, SET}
        public enum SupportedTypes { JsonObject }
        public Header MessageHeader = new Header();
        public Message MessageData = new Message();
        public Signature MessageSignature = new Signature();

        #endregion Vars

        #region Initialization
        public UniversalPacket(Header Messageheader, Message Messagedata, Signature Messagesignature)
        {
            MessageHeader = Messageheader;
            MessageData = Messagedata;
            MessageSignature = Messagesignature;
        }

        #endregion Initialization

        #region ToString
        public override string ToString() => JsonConvert.SerializeObject(MessageHeader) + "." + JsonConvert.SerializeObject(MessageData.Data) + "." + JsonConvert.SerializeObject(MessageSignature) + "." + MessageData.IsEncrypted.ToString();

        #endregion ToString
    }

    public class Header
    {
        public string type;
        public UniversalPacket.HTTPSTATUS status;
        public DateTime dateTime;
    }

    public class Message
    {
        public object Data;
        public bool IsEncrypted;
    }

    public class Signature
    {
        public string clientIp;
        public string clientId;
    }
}
