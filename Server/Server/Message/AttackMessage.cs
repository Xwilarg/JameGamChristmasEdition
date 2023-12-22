using JameGam.Common;
using System;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Message sent when an attack animation is started
    /// </summary>
    internal class AttackMessage : IMessage
    {
        public MessageType Type => MessageType.AttackAnim;

        public void Write(BinaryWriter writer)
        {
        }
    }
}
