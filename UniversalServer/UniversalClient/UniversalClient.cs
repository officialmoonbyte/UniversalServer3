using Moonbyte.Security;
using Moonbyte.UniversalServer.Core.Networking;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                    new Message { Data = JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.getserverpublickey" }), IsEncrypted = false },
                    clientSignature);
                UniversalServerPacket s = SendMessage(getServerPublicKeyRequest);
                this.Encryption.SetServerPublicKey(s.Message);

                UniversalPacket sendClientPublicKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message { 
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.setclientpublickey", 
                        Encryption.GetClientPublicKey() }, Formatting.None), Encryption.GetServerPublicKey()), IsEncrypted = true },
                    clientSignature);
                UniversalServerPacket bs = SendMessage(sendClientPublicKey);
                Console.WriteLine(bs.Message);

                UniversalPacket sendClientPrivateKey = new UniversalPacket(
                    new Header { status = UniversalPacket.HTTPSTATUS.POST },
                    new Message
                    {
                        Data = Encryption.Encrypt(
                        JsonConvert.SerializeObject(new string[] { "serverrequest.encryption.setclientprivatekey", 
                        Encryption.GetClientPrivateKey(), Encryption.GetServerPublicKey() }, Formatting.None), Encryption.GetServerPublicKey()), IsEncrypted = true },
                    clientSignature);
                UniversalServerPacket bs2 = SendMessage(sendClientPrivateKey);
                Console.WriteLine(bs2.Message);
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

        public UniversalServerPacket WaitForResult()
        {
            byte[] data = new byte[Client.Client.ReceiveBufferSize];
            int receivedDataLength = Client.Client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);

            return JsonConvert.DeserializeObject<UniversalServerPacket>(stringData);
        }

        #endregion WaitForResult

        #region SendMessage (Internal)

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
