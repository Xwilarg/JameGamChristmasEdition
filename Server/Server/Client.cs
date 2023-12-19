using System.Net.Sockets;

namespace Server
{
    /// <summary>
    /// A connected client
    /// </summary>
    /// <param name="socket">The client socket</param>
    internal class Client(Socket socket)
    {
        /// <summary>
        /// Gets the connected socket
        /// </summary>
        public Socket Socket { get; } = socket;

        /// <summary>
        /// Gets the stream of the socket
        /// </summary>
        public NetworkStream Stream { get; } = new NetworkStream(socket);

        /// <summary>
        /// Gets or sets the state of the client
        /// </summary>
        public ClientState State { get; set; } = ClientState.Connecting;

        /// <summary>
        /// Gets or sets the name of the client
        /// </summary>
        public string Name { get; set; }
    }
}
