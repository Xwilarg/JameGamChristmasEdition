using JameGam.Common;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when a client disconnects
    /// </summary>
    /// <param name="id">The ID of the disconnected client</param>
    internal class DisconnectedMessage(int id) : IMessage
    {
        public MessageType Type => MessageType.Disconnected;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
        }
    }
}
