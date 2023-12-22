using JameGam.Common;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when a client gets killed
    /// </summary>
    /// <param name="id">The ID of the killed client</param>
    internal class DeathMessage(int id) : IMessage
    {
        public MessageType Type => MessageType.Death;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
        }
    }
}
