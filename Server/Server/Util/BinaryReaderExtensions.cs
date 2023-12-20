using System.IO;
using System.Numerics;

namespace Server.Util
{
    /// <summary>
    /// Extensions for the <see cref="BinaryReader"/> class
    /// </summary>
    internal static class BinaryReaderExtensions
    {
        public static Vector2 ReadVector2(this BinaryReader writer)
        {
            return new Vector2(writer.ReadSingle(), writer.ReadSingle());
        }
    }
}
