namespace Socket.Soccer.WebAPI.Entities
{
    /// <summary>
    /// Játékos
    /// </summary>
    public class Player
    {
        
        public static readonly Position HOME_START_POSITION = new Position(10, 7, 1);
        public static readonly Position AWAY_START_POSITION = new Position(14, 9, -1);
        public Guid Id { get; set; }
        public Position Position { get; set; }
        public Team Team { get; set; }
        private Random random = new Random();
        public int KickStrength
        {
            get
            {
                return random.Next(1, 3);
            }
        }

        public int StepSize 
        {
            get
            {
                return random.Next(1, 2);
            }
        }

        public bool CanKickTheBall(Ball ball)
        {
            var xDiff = Math.Abs(Position.X - ball.Position.X);
            var yDiff = Math.Abs(Position.Y - ball.Position.Y);
            return xDiff <= 1 && yDiff <= 1;
        }
    }

}
