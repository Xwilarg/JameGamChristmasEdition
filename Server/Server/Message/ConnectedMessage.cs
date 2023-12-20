using JameGam.Common;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when a client connects
    /// </summary>
    /// <param name="id">The ID of the new client</param>
    /// <param name="name">The name of the client</param>
    internal class ConnectedMessage(int id, string name) : IMessage
    {
        public MessageType Type => MessageType.Connected;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(name);
        }
    }
}
