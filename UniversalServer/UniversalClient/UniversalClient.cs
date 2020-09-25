using Moonbyte.Security;
using Moonbyte.UniversalServer.Core.Networking;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniversalServer.Core.Networking;

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
        private ClientRSA Encryption;
        private Signature clientSignature;
        private string response;

        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        #endregion Vars

        #region Properties

        public Signature GetSignature
        {
            get 
            { 
                return clientSignature; 
            }
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (Client.Connected)
                    { return true; }
                    else
                    { return false; }
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion Properties

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

        #region ConnectToRemoteServerAsync

        public async Task ConnectToRemoteServerAsync(string ServerIP, int ServerPort)
        {
            if (Client == null) return;

            await Client.ConnectAsync(ServerIP, ServerPort);

            if (Client.Connected)
            {
                UniversalPacket getServerPublicKeyRequest = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.GET },
                    new Message { Data = JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.getserverpublickey" }), IsEncrypted = false },
                    clientSignature);
                Task<UniversalServerPacket> sendMessageTask = SendMessageAsync(getServerPublicKeyRequest);
                UniversalServerPacket s = await sendMessageTask;
                this.Encryption.SetServerPublicKey(s.Message);

                UniversalPacket sendClientPublicKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message
                    {
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.setclientpublickey",
                        Encryption.GetClientPublicKey() }, Formatting.None), Encryption.GetServerPublicKey()),
                        IsEncrypted = true
                    },
                    clientSignature);
                Task<UniversalServerPacket> sendClientPublicKeyTask = SendMessageAsync(sendClientPublicKey);
                UniversalServerPacket bs = await sendClientPublicKeyTask;

                UniversalPacket sendClientPrivateKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message
                    {
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.setclientprivatekey",
                        Encryption.GetClientPrivateKey(), Encryption.GetServerPublicKey() }, Formatting.None), Encryption.GetServerPublicKey()),
                        IsEncrypted = true
                    },
                    clientSignature);
                Task<UniversalServerPacket> sendClientPrivateKeyTask = SendMessageAsync(sendClientPrivateKey);
                UniversalServerPacket bs2 = await sendClientPrivateKeyTask;
            }
        }

        #endregion ConnectToRemoteServerAsync

        #region ConnectToRemoteServer

        public void ConnectToRemoteServer(string ServerIP, int ServerPort)
        {
            if (Client == null) return;

            Client.Connect(ServerIP, ServerPort);

            if (Client.Connected)
            {
                UniversalPacket getServerPublicKeyRequest = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.GET },
                    new Message { Data = JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.getserverpublickey" }), IsEncrypted = false },
                    clientSignature);
                UniversalServerPacket s = SendMessage(getServerPublicKeyRequest);
                this.Encryption.SetServerPublicKey(s.Message);

                UniversalPacket sendClientPublicKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message
                    {
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.setclientpublickey",
                        Encryption.GetClientPublicKey() }, Formatting.None), Encryption.GetServerPublicKey()),
                        IsEncrypted = true
                    },
                    clientSignature);
                UniversalServerPacket bs = SendMessage(sendClientPublicKey);

                UniversalPacket sendClientPrivateKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message
                    {
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.setclientprivatekey",
                        Encryption.GetClientPrivateKey(), Encryption.GetServerPublicKey() }, Formatting.None), Encryption.GetServerPublicKey()),
                        IsEncrypted = true
                    },
                    clientSignature);
                UniversalServerPacket bs2 = SendMessage(sendClientPrivateKey);
            }
        }

        #endregion ConnectToRemoteServer

        #region WaitForResult

        public async Task<UniversalServerPacket> WaitForResultAsync()
        {
            UniversalServerPacket serverPacket = null;
            await Task.Run(() =>
            {
                byte[] data = new byte[Client.Client.ReceiveBufferSize];
                int receivedDataLength = Client.Client.Receive(data);
                serverPacket = JsonConvert.DeserializeObject<UniversalServerPacket>
                               (Encoding.ASCII.GetString(data, 0, receivedDataLength));

                if (serverPacket.Encrypted)
                {
                    serverPacket.Message = Encryption.Decrypt(serverPacket.Message, Encryption.GetClientPrivateKey());
                }
            });

            return serverPacket;
        }

        public UniversalServerPacket WaitForResult()
        {
            UniversalServerPacket serverPacket;

            byte[] data = new byte[Client.Client.ReceiveBufferSize];
            int receivedDataLength = Client.Client.Receive(data);
            serverPacket = JsonConvert.DeserializeObject<UniversalServerPacket>
                           (Encoding.ASCII.GetString(data, 0, receivedDataLength));

            if (serverPacket.Encrypted)
            {
                serverPacket.Message = Encryption.Decrypt(serverPacket.Message, Encryption.GetClientPrivateKey());
            }

            return serverPacket;
        }

        #endregion WaitForResult

        #region SendMessage (Internal)

        public async Task<UniversalServerPacket> SendMessageAsync(UniversalPacket packet)
        {
            await Task.Run(() =>
            {
                string s = packet.ToString() + "<EOF>";
                byte[] BytesToSend = Encoding.UTF8.GetBytes(packet.ToString() + "<EOF>");
                Client.Client.BeginSend(BytesToSend, 0, BytesToSend.Length, 0, new AsyncCallback(SendCallBack), Client);
            });

            return await WaitForResultAsync();
        }

        public UniversalServerPacket SendMessage(UniversalPacket packet)
        {
            string s = packet.ToString() + "<EOF>";
            byte[] BytesToSend = Encoding.UTF8.GetBytes(packet.ToString() + "<EOF>");
            Client.Client.BeginSend(BytesToSend, 0, BytesToSend.Length, 0, new AsyncCallback(SendCallBack), Client);

            return WaitForResult();
        }

        #endregion SendMessage (Internal)

        #region SendCallBack

        private void SendCallBack(IAsyncResult ar)
        {

        }

        #endregion SendCallBack

        #region Encrypt

        public string Encrypt(string Data) => Encryption.Encrypt(Data, Encryption.GetClientPublicKey());
        

        #endregion Encrypt

        #region Disconnect

        public void Disconnect()
        {
            if (Client.Connected) Client.Close();
        }

        #endregion Disconnect

        public void Dispose()
        {
            Client = null;
            Encryption = null;
            clientSignature = null;
        }
    }
}
