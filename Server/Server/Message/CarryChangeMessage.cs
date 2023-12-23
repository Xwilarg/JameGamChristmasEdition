using JameGam.Common;
using System;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when the item carried by a player changes
    /// </summary>
    internal class CarryChangeMessage(int id, short carry) : IMessage
    {
        public MessageType Type => MessageType.CarryChange;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(carry);
        }
    }
}
