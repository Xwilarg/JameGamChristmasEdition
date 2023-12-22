using JameGam.Common;
using System.IO;

namespace Server.Message
{
    internal class GameResetMessage() : IMessage
    {
        public MessageType Type => MessageType.Death;

        public void Write(BinaryWriter writer)
        { }
    }
}
