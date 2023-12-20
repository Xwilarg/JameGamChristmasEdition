using JameGam.Common;
using Server.Util;
using System.IO;
using System.Numerics;

namespace Server.Message
{
    /// <summary>
    /// Message sent with spacial info about a client
    /// </summary>
    /// <param name="id">The ID of the client</param>
    /// <param name="position">The client position</param>
    /// <param name="velocity">The client velocity</param>
    internal class SpacialMessage(int id, Vector2 position, Vector2 velocity) : IMessage
    {
        public MessageType Type => MessageType.SpacialInfo;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(position);
            writer.Write(velocity);
        }
    }
}
