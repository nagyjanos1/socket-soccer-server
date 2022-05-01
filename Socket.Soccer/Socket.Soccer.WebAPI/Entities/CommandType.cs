namespace Socket.Soccer.WebAPI.Entities
{
    public enum CommandType
    {
        /// <summary>
        /// Mozgás
        /// </summary>
        Move,

        /// <summary>
        /// Rúgás
        /// </summary>
        Kick,

        /// <summary>
        /// Passzolás
        /// </summary>
        Pass,

        /// <summary>
        /// Cselezi a labdát
        /// </summary>
        Trick
    }

}
