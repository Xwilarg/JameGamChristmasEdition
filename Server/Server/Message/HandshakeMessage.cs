using JameGam.Common;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent after a handshake
    /// </summary>
    /// <param name="id">The assigned ID for the client</param>
    internal class HandshakeMessage(int id) : IMessage
    {
        public MessageType Type => MessageType.Handshake;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
        }
    }
}
