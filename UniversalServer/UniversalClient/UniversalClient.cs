using Moonbyte.Security;
using Moonbyte.UniversalServer.Core.Networking;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Moonbyte.Networking
{

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class UniversalClient : IDisposable
    {

        #region Vars

        private TcpClient Client;
        ClientRSA Encryption;
        Signature clientSignature;

        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private static String response = String.Empty;

        public bool IsConnected
        {
            get
            { try { if (Client.Connected) { return true; } else { return false; } } catch { return false; } }
        }

        #endregion Vars

        #region Initialization

        public UniversalClient()
        {
            Client = new TcpClient();
            Encryption = new ClientRSA();
            clientSignature = new Signature();
            clientSignature.clientId = FingerPrint.Value();
            clientSignature.clientIp = new WebClient().DownloadString("http://icanhazip.com");
        }

        #endregion Intialization

        #region ConnectToRemoteServer

        public void ConnectToRemoteServer(string ServerIP, int ServerPort)
        {
            if (Client == null) return;

            Client.Connect(ServerIP, ServerPort);

            if (Client.Connected)
            {
                UniversalPacket getServerPublicKeyRequest = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.GET },
                    new Message { Data = JsonConvert.SerializeObject(new string[] { "servercommand", "encryption", "getserverpublickey" }), IsEncrypted = false },
                    clientSignature);
                string s = SendMessage(getServerPublicKeyRequest);
                this.Encryption.SetServerPublicKey(s);

                UniversalPacket sendClientPublicKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message { 
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "servercommand", "encryption", "setclientpublickey", Encryption.GetClientPublicKey() }),
                        Encryption.GetServerPublicKey()), IsEncrypted = true },
                    clientSignature);
                SendMessage(sendClientPublicKey);

                UniversalPacket sendClientPrivateKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message
                    {
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "servercommand", "encryption", "setclientprivatekey", Encryption.GetClientPrivateKey() }),
                        Encryption.GetServerPublicKey()),
                        IsEncrypted = true
                    },
                    clientSignature);
                SendMessage(sendClientPrivateKey);
            }
        }

        #endregion ConnectToRemoteServer

        #region WaitForResult

        public void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                }

                receiveDone.Set();
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string WaitForResult()
        {
            byte[] data = new byte[Client.Client.ReceiveBufferSize];
            int receivedDataLength = Client.Client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            string Final = stringData.Replace("%20%", " ");

            return Final;
        }

        #endregion WaitForResult

        #region SendCommand

        public string SendCommand(string Command, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace(" ", "%20%");
            }
            string ArgsSend = string.Join(" ", args);
            string valueToSend = Command + " " + ArgsSend;
            SendMessage(valueToSend);
            return WaitForResult();
        }

        #endregion SendCommand

        #region SendMessage (Internal)

        private string SendMessage(IUniversalPacket packet)
        {
            byte[] BytesToSend = Encoding.UTF8.GetBytes(packet.ToString() + "<EOF>");
            Client.Client.BeginSend(BytesToSend, 0, BytesToSend.Length, 0, new AsyncCallback(SendCallBack), Client);
            return WaitForResult();
        }

        #endregion SendMessage (Internal)

        #region SendMessage

        public void SendMessage(string Value, bool UseEncryption = true)
        {
            if (UseEncryption) { Value = Encryption.Encrypt(Value, Encryption.GetServerPublicKey()); }
            UniversalPacket packet = new UniversalPacket(
                new Header() { type = Value.GetType().ToString(), dateTime = new DateTime(), status = UniversalPacket.HTTPSTATUS.GET },
                new Message() { IsEncrypted = UseEncryption, Data = Value },
                new Signature() { clientId = FingerPrint.Value(), clientIp = new WebClient().DownloadString("http://icanhazip.com") });

        }

        #endregion SendMessage

        #region SendCallBack

        private void SendCallBack(IAsyncResult ar)
        {

        }

        #endregion SendCallBack

        #region Disconnect

        public void Disconnect()
        {
            if (Client.Connected) Client.Close();
        }

        #endregion Disconnect

        public void Dispose()
        {
            
        }
    }
}
