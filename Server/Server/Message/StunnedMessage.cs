using JameGam.Common;
using System.IO;
using System.Numerics;

namespace Server.Message
{
    /// <summary>
    /// Message sent when an attack animation is started
    /// </summary>
    internal class StunnedMessage(int id, Vector2 dir) : IMessage
    {
        public MessageType Type => MessageType.Stunned;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(dir);
        }
    }
}
