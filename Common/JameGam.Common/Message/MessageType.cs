namespace JameGam.Common
{
    /// <summary>
    /// The type of message sent
    /// </summary>
    public enum MessageType : ushort
    {
        /// <summary>
        /// Sent by the client after connecting
        /// </summary>
        Handshake,

        /// <summary>
        /// Sent by the server when a client has joined
        /// </summary>
        Connected,

        /// <summary>
        /// Sent by the server when a client has left
        /// </summary>
        Disconnected,

        /// <summary>
        /// Sent by the client and server with spacial info about a client 
        /// </summary>
        SpacialInfo
    }
}