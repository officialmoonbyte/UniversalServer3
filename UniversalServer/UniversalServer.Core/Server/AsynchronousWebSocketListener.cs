/*using System;
using System.Text;

namespace Moonbyte.UniversalServer.Core
{
    public class AsynchronousWebSocketListener
    {

        #region Initialization

        public AsynchronousWebSocketListener(string IP, int Port)
        {
            WatsonWsServer server = new WatsonWsServer(IP, Port, true | false);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;
            server.Start();
        }

        #endregion Initialization

        #region Message Receivers 

        static void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(args.Data));
        }

        static void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Console.WriteLine("Client connected");
        }

        static void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Console.WriteLine("Server disconnected");
        }

        #endregion Message Receivers
    }
}
*/