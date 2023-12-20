using System.IO;
using System.Numerics;

namespace Server.Util
{
    /// <summary>
    /// Extensions for <see cref="BinaryReader"/> and <see cref="BinaryWriter"/> classes
    /// </summary>
    internal static class BinaryExtensions
    {
        /// <summary>
        /// Reads a <see cref="Vector2"/> from the stream
        /// </summary>
        /// <param name="writer">The reader</param>
        /// <returns>The read vector</returns>
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

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
