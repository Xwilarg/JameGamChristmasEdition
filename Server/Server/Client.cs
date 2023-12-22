using JameGam.Common;
using Server.Message;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Numerics;

namespace Server
{
    /// <summary>
    /// A connected client
    /// </summary>
    /// <param name="socket">The client socket</param>
    internal class Client(Socket socket, GameServer gameServer)
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
        /// Gets the game server instance associated with this client
        /// </summary>
        public GameServer GameServer { get; } = gameServer;

        /// <summary>
        /// Gets or sets the ID of the client
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the client
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the position of the client
        /// </summary>
        public Vector2 Position { get; set; }

        public bool IsDead { get; set; } = true;

        public void Reset()
        {
            IsDead = false;
        }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="type">The message type</param>
        /// <param name="message">The message data</param>
        public void SendMessage(MessageType type, byte[] message)
        {
            Debug.WriteLine($"S->{Id} {type}");

            var data = new byte[message.Length + 2];
            var bytes = BitConverter.GetBytes((ushort)type);

            // Write 2 bytes message type and add data after
            Array.Copy(bytes, data, 2);
            Array.Copy(message, 0, data, 2, message.Length);

            try
            {
                Stream.Write(data);
            }
            catch (IOException)
            {
                Console.WriteLine($"Connection dropped with {Id}");
                GameServer.RemoveClient(this);
            }
        }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="message">The message</param>
        public void SendMessage(IMessage message)
        {
            // Write the message to a buffer
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            message.Write(writer);
            var data = stream.ToArray();

            // Send the message data to the client
            SendMessage(message.Type, data);
        }
    }
}
