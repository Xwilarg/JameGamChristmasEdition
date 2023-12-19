namespace JameGam.Common
{
    public enum MessageType : short
    {
        /// <summary>
        /// Sent by the client after connecting
        /// </summary>
        Handshake,

        Connected,
        Disconnected,

        SpacialInfo
    }
}