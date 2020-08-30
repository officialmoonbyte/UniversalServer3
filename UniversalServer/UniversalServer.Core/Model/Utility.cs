using Moonbyte.UniversalServer.Core.Client;
using Moonbyte.UniversalServer.Core.Server;
using System;
using System.Text;

namespace Moonbyte.UniversalServer.Core.Model
{
    public static class Utility
    {
        public enum MoonbyteCancelRequest { Cancel, Continue }
        public static string[] bytesToStringArray(ClientWorkObject workObject, int bytesRead, AsynchronousSocketListener listener)
        {
            string[] returnStringArray = null;
            //int bytesRead = workObject.clientSocket.EndReceive(result);

            if (bytesRead > 0)
            {
                workObject.sb.Append(Encoding.ASCII.GetString(workObject.buffer, 0, bytesRead));

                string content = workObject.sb.ToString();

                if (content.IndexOf("<EOF>") > -1)
                {
                    workObject.sb = new StringBuilder();
                    workObject.buffer = new byte[ClientWorkObject.BufferSize];

                    returnStringArray = content.Split(new string[] { "|SPLT|", "<EOF>" },
                                    StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    listener.ClientBeginReceive(workObject);
                }
            }

            return returnStringArray;
        }
    }
}
