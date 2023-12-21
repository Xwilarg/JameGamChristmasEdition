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
        SpacialInfo,

        /// <summary>
        /// Sent by the client and server when a player die in-game
        /// </summary>
        Death,

        /// <summary>
        /// Sent by the client and server when an attack animation is played
        /// </summary>
        AttackAnim,

        /// <summary>
        /// Sent by the client and server when the item carried in-game by a player change
        /// </summary>
        CarryChange,
    }
}