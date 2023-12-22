using JameGam.Common;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when a game ends
    /// </summary>
    internal class GameResetMessage : IMessage
    {
        public MessageType Type => MessageType.ResetGame;

        public void Write(BinaryWriter writer)
        {
        }
    }
}
