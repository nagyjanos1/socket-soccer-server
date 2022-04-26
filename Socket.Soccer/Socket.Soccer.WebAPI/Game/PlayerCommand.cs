namespace Socket.Soccer.WebAPI.Game
{
    /// <summary>
    /// Egy darab játékos műveletét reprezentálja
    /// </summary>
    public class PlayerCommand
    {
        public Guid PlayerId { get; set; }
        public Direction Direction { get; set; }
        public CommandType Command { get; set; }
    }
}
