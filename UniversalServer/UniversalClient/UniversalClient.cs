using System;
using System.Net.Sockets;
using System.Text;
using UniversalClient.Security;

namespace UniversalClient
{
    public class Universalclient
    {

        #region Vars

        private TcpClient Client;
        ClientRSA Encryption = new ClientRSA();
        public static bool LogEvents;

        public bool IsConnected
        {
            get
            { try { if (Client.Connected) { return true; } else { return false; } } catch { return false; } }
        }

        #endregion Vars

        #region Initialization

        public Universalclient(bool logEvents)
        {
            LogEvents = logEvents;
            Client = new TcpClient();
        }

        #endregion Intialization

        #region ConnectToRemoteServer

        public void ConnectToRemoteServer(string ServerIP, int ServerPort)
        {
            if (Client == null) return;

            Client.Connect(ServerIP, ServerPort);

            if (Client.Connected)
            {
                SendMessage("Key_ServerPublic|", false, null);
                this.Encryption.SetServerPublicKey(WaitForResult());
                try
                {
                    string encryptedClientPublicKey = Encryption.Encrypt(Encryption.GetClientPublicKey(), Encryption.GetServerPublicKey());
                    Console.WriteLine("YEET ::: " + encryptedClientPublicKey);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine("Yah ::: " + Encryption.GetServerPublicKey());
                //SendMessage("Key_ClientPublic|" + this.Encryption.Encrypt(Encryption.GetClientPublicKey(), Encryption.GetServerPublicKey()), false); WaitForResult();
                //SendMessage("Key_ClientPrivate|" + this.Encryption.Encrypt(Encryption.GetClientPrivateKey(), Encryption.GetServerPublicKey()), false); WaitForResult();
            }
        }

        #endregion ConnectToRemoteServer

        #region WaitForResult

        public string WaitForResult(bool UseEncryption = false)
        {
            byte[] data = new byte[Client.Client.ReceiveBufferSize];
            int receivedDataLength = Client.Client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            if (LogEvents) Console.WriteLine("Server response: " + stringData);
            string Final = stringData.Replace("%20%", " ");
            if (UseEncryption)
            { Final = Encryption.Decrypt(Final, Encryption.GetClientPrivateKey()); }
            Console.WriteLine(Final);
            return Final;
        }

        #endregion WaitForResult

        #region SendCommand

        public string SendCommand(string Command, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace(" ", "%40%");
            }
            string ArgsSend = string.Join(" ", args);
            if (LogEvents) Console.WriteLine("Args Send : " + ArgsSend);
            string valueToSend = "CLNT|" + Command + " " + ArgsSend;
            SendMessage(valueToSend);
            return WaitForResult(true);
        }

        #endregion SendCommand

        #region SendMessage

        public void SendMessage(string Value, bool UseEncryption = true, string Key = null)
        {
            //Sends the message to the client
            string stringToSend = Value.Replace(" ", "%20%");
            if (UseEncryption)
            {
                if (Key == null) { stringToSend = Encryption.Encrypt(stringToSend, Encryption.GetClientPrivateKey()); }
                else { stringToSend = Encryption.Encrypt(stringToSend, Key); }
            }
            if (LogEvents) Console.WriteLine("Sending " + stringToSend);
            stringToSend += "<EOF>";
            byte[] BytesToSend = Encoding.UTF8.GetBytes(stringToSend);
            Client.Client.BeginSend(BytesToSend, 0, BytesToSend.Length, 0, new AsyncCallback(SendCallBack), Client);
        }

        #endregion SendMessage

        #region SendCallBack

        private void SendCallBack(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                if (LogEvents) Console.WriteLine("Data sent sucessfully!");
            }
            else
            {
                if (LogEvents) Console.WriteLine("Data was not sucessfully!");
            }
        }

        #endregion SendCallBack

        #region Disconnect

        public void Disconnect()
        {
            if (Client.Connected) Client.Close();
        }

        #endregion Disconnect
    }
}
