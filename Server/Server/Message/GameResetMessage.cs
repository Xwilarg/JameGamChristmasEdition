using JameGam.Common;
using System.IO;

namespace Server.Message
{
    internal class GameResetMessage() : IMessage
    {
        public MessageType Type => MessageType.ResetGame;

        public void Write(BinaryWriter writer)
        { }
    }
}
