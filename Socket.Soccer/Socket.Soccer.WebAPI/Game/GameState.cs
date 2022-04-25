namespace Socket.Soccer.WebAPI.Game
{

    /// <summary>
    /// A játék elemeit és állapotát tartalmazza
    /// </summary>
    public class GameState
    {
        public Ball Ball { get; set; }
        public List<Player> Players { get; set; } = new List<Player>(22);
    }

}
