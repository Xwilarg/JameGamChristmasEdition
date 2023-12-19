using System.Net.Sockets;

namespace Server
{
    internal class Client(Socket socket)
    {
        public Socket Socket { get; set; } = socket;
        public ClientState State { get; set; } = ClientState.Connecting;
    }
}
