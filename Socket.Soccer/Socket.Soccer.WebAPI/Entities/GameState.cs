namespace Socket.Soccer.WebAPI.Entities
{
    public class GameState
    {
        public int HomeScores { get; set; } = 0;
        public int AwayScores { get; set; } = 0;
        public Team? IsGoal { get; set; } = null;
        public bool IsBallOut { get; set; } = false;
    }

}
