using System.Net.Sockets;

namespace Communicator.Common.Entities
{
    public class ConnectedClient
    {
        public TcpClient Socket { get; set; }

        public string UserName { get; set; }

        public PublicKey PublicKey { get; set; }
    }
}
