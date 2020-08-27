﻿using Newtonsoft.Json;
using System;

namespace UniversalServer.Core.Networking
{
    public class UniversalPacket : IUniversalPacket
    {

        #region Vars
        public enum HTTPSTATUS { GET, SET}
        public enum SupportedTypes { JsonObject }
        public readonly string SplitString = "|SPLT|";
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
        public override string ToString() => JsonConvert.SerializeObject(MessageHeader) + SplitString 
            + JsonConvert.SerializeObject(MessageData.Data) + SplitString 
            + JsonConvert.SerializeObject(MessageSignature) + SplitString 
            + MessageData.IsEncrypted.ToString();

        public string GetSplitString() => SplitString;
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
