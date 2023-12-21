using JameGam.Common;
using System.IO;

namespace Server.Message
{
    internal class HandshakeMessage(int id) : IMessage
    {
        public MessageType Type => MessageType.Handshake;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
        }
    }
}
