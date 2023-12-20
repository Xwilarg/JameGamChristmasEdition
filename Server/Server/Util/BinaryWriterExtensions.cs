using System.IO;
using System.Numerics;

namespace Server.Util
{
    /// <summary>
    /// Extensions for the <see cref="BinaryWriter"/> class
    /// </summary>
    internal static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes a <see cref="Vector3"/> to the stream 
        /// </summary>
        /// <param name="writer">The writer</param>
        /// <param name="vector">The vector</param>
        public static void Write(this BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }
    }
}
