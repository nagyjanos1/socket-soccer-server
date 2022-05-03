namespace Socket.Soccer.WebAPI.Entities
{

    /// <summary>
    /// A játék elemeit és állapotát tartalmazza
    /// </summary>
    public class Game
    {
        public Ball Ball { get; set; } = new Ball();
        public List<Player> Players { get; set; } = new List<Player>(22);
        public FootballPit Pit { get; set; } = new FootballPit();
        public GameState State { get; set; } = new GameState();

        public void ResetState()
        {
            this.State.IsGoal = null;
            this.State.IsBallOut = false;
            this.Ball.Position = this.Ball.BALL_START_POSITION;
            foreach (var player in this.Players)
            {
                player.Position = player.Team == Team.Home ? Player.HOME_START_POSITION : Player.AWAY_START_POSITION;
            }
        }
    }

}
