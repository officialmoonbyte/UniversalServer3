﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UniversalClient.Security;

namespace UniversalClient
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

    public class Universalclient
    {

        #region Vars

        private TcpClient Client;
        ClientRSA Encryption;
        public static bool LogEvents;

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

        public Universalclient(bool logEvents)
        {
            LogEvents = logEvents;
            Client = new TcpClient();
            Encryption = new ClientRSA();
            Console.WriteLine(Encryption.GetClientPublicKey());
        }

        #endregion Intialization

        #region ConnectToRemoteServer

        public void ConnectToRemoteServer(string ServerIP, int ServerPort)
        {
            if (Client == null) return;

            Client.Connect(ServerIP, ServerPort);

            if (Client.Connected)
            {
                SendMessage("Key_ServerPublic ", false);
                this.Encryption.SetServerPublicKey(WaitForResult());
                try
                {
                    string encryptedClientPublicKey = Encryption.Encrypt(Encryption.GetClientPublicKey(), Encryption.GetServerPublicKey());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine("Yah ::: " + Encryption.GetServerPublicKey());
                SendMessage("Key_ClientPublic " + this.Encryption.Encrypt(Encryption.GetClientPublicKey(), Encryption.GetServerPublicKey()), false); WaitForResult();
                SendMessage("Key_ClientPrivate " + this.Encryption.Encrypt(Encryption.GetClientPrivateKey(), Encryption.GetServerPublicKey()), false); WaitForResult();

                //Sends in the user data now, gets the user data encrypted first
                SendMessage("User SetID "); string loginCheck = WaitForResult();
                if (loginCheck.ToUpper() != "TRUE")
                {
                    Console.WriteLine("Failed to authorize user connection! Closing and disposing client object.");
                    this.Disconnect(); this.Dispose();
                }
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
            if (LogEvents) Console.WriteLine("Server response: " + stringData);
            string Final = stringData.Replace("%20%", " ");
            string[] splitFinal = Final.Split('|');
            if (splitFinal[0] == true.ToString())
            { Final = Encryption.Decrypt(splitFinal[1], Encryption.GetClientPrivateKey()); }
            else { Final = splitFinal[1]; }
            Console.WriteLine("Final : " + Final);

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
            if (LogEvents) Console.WriteLine("Args Send : " + ArgsSend);
            string valueToSend = Command + " " + ArgsSend;
            SendMessage(valueToSend);
            return WaitForResult();
        }

        #endregion SendCommand

        #region SendMessage

        public void SendMessage(string Value, bool UseEncryption = true)
        {
            //Sends the message to the client
            string stringToSend = Value.Replace(" ", "%20%");
            if (UseEncryption)
            { stringToSend = Encryption.Encrypt(stringToSend, Encryption.GetServerPublicKey()); }

            string Header = UseEncryption.ToString() + "%20%" + FingerPrint.Value() + "|";
            stringToSend = Header + stringToSend;

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

        #region Dispose

        public void Dispose()
        {

        }

        #endregion Dispose
    }
}
