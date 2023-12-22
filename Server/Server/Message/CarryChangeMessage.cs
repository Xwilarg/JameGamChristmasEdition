using JameGam.Common;
using System;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when the item carried by a player changes
    /// </summary>
    internal class CarryChangeMessage : IMessage
    {
        public MessageType Type => MessageType.CarryChange;

        public void Write(BinaryWriter writer)
        {
        }
    }
}
