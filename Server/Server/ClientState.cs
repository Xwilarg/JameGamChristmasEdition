namespace Server
{
    /// <summary>
    /// The state of the client
    /// </summary>
    internal enum ClientState
    {
        /// <summary>
        /// The client established a connection but has not sent a handshake
        /// </summary>
        Connecting,

        /// <summary>
        /// The client is connected
        /// </summary>
        Connected
    }
}
