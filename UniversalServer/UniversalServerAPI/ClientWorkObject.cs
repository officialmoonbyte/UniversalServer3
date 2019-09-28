using System.Net.Sockets;
using System.Text;

namespace Moonbyte.UniversalServerAPI
{
    public class ClientWorkObject
    {
        #region Network Objects

        public Socket clientSocket = null;

        #endregion Network Objects

        #region Tracking Objects / other services



        #endregion Tracking Objects

        #region Buffer

        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];

        #endregion Buffer

        #region Other

        public StringBuilder sb = new StringBuilder();

        #endregion Other
    }
}
