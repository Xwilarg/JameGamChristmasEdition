using JameGam.Common;
using System.IO;

namespace Server.Message
{
    /// <summary>
    /// Interface to be implemented by messages
    /// </summary>
    internal interface IMessage
    {
        /// <summary>
        /// Gets the message type
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="writer">The writer to write to</param>
        public void Write(BinaryWriter writer);
    }
}
