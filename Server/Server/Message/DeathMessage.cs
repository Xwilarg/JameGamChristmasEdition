using JameGam.Common;
using System.IO;

namespace Server.Message
{
    internal class DeathMessage(int id) : IMessage
    {
        public MessageType Type => MessageType.Death;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
        }
    }
}
