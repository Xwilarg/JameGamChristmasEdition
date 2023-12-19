using System.Net.Sockets;

namespace Server
{
    internal class Client(Socket socket)
    {
        public Socket Socket { get; set; } = socket;
        public NetworkStream Stream { get; set; } = new NetworkStream(socket);

        public ClientState State { get; set; } = ClientState.Connecting;
        public string Name { get; set; }
    }
}
